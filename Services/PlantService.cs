using System.Net.Http.Json;

namespace FirstMauiApp.Services;

public class PlantService
{
    HttpClient httpClient;
    public PlantService()
    {
        httpClient = new HttpClient();
    }

    private bool IsInternetConnected()
    {
        var currentNetworkStatus = Connectivity.NetworkAccess;
        return currentNetworkStatus == NetworkAccess.Internet;
    }

    List<Plant> plantList = new();

    public async Task<List<Plant>> GetPlants()
    {
        if (plantList?.Count > 0)
            return plantList;

        if (IsInternetConnected())
        {
            // Online
            var response = await httpClient.GetAsync("https://raw.githubusercontent.com/joel-mainey/BCA-Maui-App/master/Resources/Raw/plantdata.json");
            if (response.IsSuccessStatusCode)
            {
                plantList = await response.Content.ReadFromJsonAsync<List<Plant>>();
                return plantList;
            }
        }

        // Offline
        using var stream = await FileSystem.OpenAppPackageFileAsync("plantdata.json");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        plantList = JsonSerializer.Deserialize<List<Plant>>(contents);
 
        return plantList;
    }
}
