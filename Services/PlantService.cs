using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace FirstMauiApp.Services;

public class PlantService
{
    HttpClient httpClient;
    public PlantService()
    {
        httpClient = new HttpClient();
    }

    List<Plant> plantList = new();

    object lockObject = new();

    public async Task<List<Plant>> GetPlants()
    {
        if (plantList.Count > 0)
            return plantList;

        // Online
        var url = "https://raw.githubusercontent.com/joel-mainey/PlantDataJSON/main/backup%20-%20formatted%20-%20selective.json";

        try
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var onlinePlants = await response.Content.ReadFromJsonAsync<List<Plant>>();

                lock (lockObject)
                {
                    plantList.AddRange(onlinePlants);
                }

                return plantList;
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error during online JSON deserialization: {ex.Message}");
        }


        // Offline
        try
        {
            var offlineJsonFile = "backup - formatted - selective.json";
            var stream = await FileSystem.OpenAppPackageFileAsync(offlineJsonFile);
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            var offlinePlants = JsonSerializer.Deserialize<List<Plant>>(contents);

            lock (lockObject)
            {
                plantList.AddRange(offlinePlants);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during offline JSON deserialization: {ex.Message}");
        }

        return plantList;
    }
}
