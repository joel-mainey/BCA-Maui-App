namespace FirstMauiApp.ViewModel;

[QueryProperty("Plant", "Plant")]
public partial class PlantsDetailsViewModel : BaseViewModel
{
    IMap map;

    public PlantsDetailsViewModel(IMap map) 
    {
        this.map = map;
    }

    [ObservableProperty]
    Plant plant;

    [RelayCommand]
    async Task OpenMapAsync()
    {
        try
        {
            await map.OpenAsync(Plant.Latitude, Plant.Longitude,
                new MapLaunchOptions
                {
                    Name = Plant.Name,
                    NavigationMode = NavigationMode.None
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to open map: {ex.Message}", "OK");
        }
    }
}
