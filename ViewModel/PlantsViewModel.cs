using FirstMauiApp.Services;
using FirstMauiApp.View;
using System.Linq;

namespace FirstMauiApp.ViewModel;

public partial class PlantsViewModel : BaseViewModel
{
    public ObservableCollection<Plant> Plants { get; } = new();

    PlantService plantService;
    IConnectivity connectivityService;
    IGeolocation geolocationService;

    public PlantsViewModel(PlantService plantService, IConnectivity connectivityService, IGeolocation geolocationService)
    {
        Title = "Tiwaiwaka Plant Finder";
        this.plantService = plantService;
        this.connectivityService = connectivityService;
        this.geolocationService = geolocationService;
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
            var location = await geolocationService.GetLastKnownLocationAsync();
            if (location is null)
            {
                location = await geolocationService.GetLocationAsync(
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
            //if (connectivityService.NetworkAccess != NetworkAccess.Internet)
            //{
            //    await Shell.Current.DisplayAlert("No connectivity!",
            //        $"Please check internet and try again.", "OK");
            //    return;
            //}

            IsBusy = true;
            var plantGroups = await plantService.GetPlantsGroupedByLocation();

            if (Plants.Count != 0)
                Plants.Clear();

            foreach (var plantGroup in plantGroups)
            {
                foreach (var plant in plantGroup.Plants)
                {
                    Plants.Add(plant);
                }
            }
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


    //[RelayCommand]
    //async Task GoToHomePage()
    //{
    //    try
    //    {
    //        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        await Shell.Current.DisplayAlert("Error!", $"Unable to navigate to Home Page: {ex.Message}", "OK");
    //    }
    //}
}
