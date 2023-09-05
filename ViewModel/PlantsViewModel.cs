using FirstMauiApp.Services;
using FirstMauiApp.View;
using System.Linq;

namespace FirstMauiApp.ViewModel;

public partial class PlantsViewModel : BaseViewModel
{
    PlantService plantService;
    public ObservableCollection<Plant> Plants { get; } = new();

    IConnectivity connectivity;
    IGeolocation geolocation;

    public PlantsViewModel(PlantService plantService, IConnectivity connectivity, IGeolocation geolocation) 
    {
        Title = "Plant Finder";
        this.plantService = plantService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;
    }

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    async Task GetClosestPlantAsync()
    {
        if (IsBusy || Plants.Count == 0)
            return;

        try
        {
            var location = await geolocation.GetLastKnownLocationAsync();
            if (location is null)
            {
                location = await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30),
                    });
            }

            if (location is null)
                return;

            var first = Plants.OrderBy(m =>
                location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Kilometers)
                ).FirstOrDefault();

            if (first is null)
                return;

            await Shell.Current.DisplayAlert("Closest Plant",
                $"{first.Name} in {first.Location}", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get closest plant: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task GoToDetailsAsync(Plant plant)
    {
        if (plant == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
            new Dictionary<string, object>
            {
                {"Plant", plant}
            });
    }

    [RelayCommand]
    async Task GetPlantsAsync()
    {
        if (IsBusy) 
            return;

        try
        {
            IsBusy = true;
            var plants = await plantService.GetPlants();

            if (Plants.Count != 0)
                Plants.Clear();

            foreach(var plant in plants)
                Plants.Add(plant); 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get plants: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
