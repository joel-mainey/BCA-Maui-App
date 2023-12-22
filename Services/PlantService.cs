using System.Net.Http.Json;
using System.IO;
using System.Text.Json;

namespace FirstMauiApp.Services
{
    public class PlantService
    {
        HttpClient httpClient;
        List<Plant> plantList = new();

        public PlantService()
        {
            httpClient = new HttpClient();
        }

        private bool IsInternetConnected()
        {
            var currentNetworkStatus = Connectivity.NetworkAccess;
            return currentNetworkStatus == NetworkAccess.Internet;
        }

        public async Task<List<PlantGroup>> GetPlantsGroupedByLocation()
        {
            if (plantList?.Count > 0)
                return GroupPlantsByLocation(plantList);

            if (IsInternetConnected())
            {
                // Online
                var response = await httpClient.GetAsync("https://raw.githubusercontent.com/joel-mainey/BCA-Maui-App/blob/Rollback/Resources/Raw/plantdata.json");
                if (response.IsSuccessStatusCode)
                {
                    plantList = await response.Content.ReadFromJsonAsync<List<Plant>>();
                    return GroupPlantsByLocation(plantList);
                }
            }

            // Offline
            using var stream = await FileSystem.OpenAppPackageFileAsync("plantdata.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            plantList = JsonSerializer.Deserialize<List<Plant>>(contents);

            return GroupPlantsByLocation(plantList);
        }

        private List<PlantGroup> GroupPlantsByLocation(List<Plant> plants)
        {
            // Group plants by location
            var groupedPlants = plants.GroupBy(p => p.Location)
                                     .Select(g => new PlantGroup
                                     {
                                         Location = g.Key,
                                         Plants = g.ToList()
                                     })
                                     .OrderBy(g => g.Location)
                                     .ToList();

            return groupedPlants;
        }
    }

    public class PlantGroup
    {
        public string Location { get; set; }
        public List<Plant> Plants { get; set; }
    }
}