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
        //if (plantList.Count > 0)
        //    return plantList;

        // Online
        //var url = "https://raw.githubusercontent.com/joel-mainey/PlantDataJSON/main/backup%20-%20formatted%20-%20selective.json";

        //var response = await httpClient.GetAsync(url);

        //if (response.IsSuccessStatusCode)
        //{
        //    var onlinePlants = await response.Content.ReadFromJsonAsync<List<Plant>>();
        //}

        // Offline
        using var stream = await FileSystem.OpenAppPackageFileAsync("plantdata.json");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        plantList = JsonSerializer.Deserialize<List<Plant>>(contents);
 
        return plantList;
    }
}
