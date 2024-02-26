namespace FirstMauiApp.View;

public partial class HomePage : ContentPage
{
    public HomePage()
	{
		InitializeComponent();

        var viewModel = new HomeViewModel();
        BindingContext = viewModel;

        // Log the ViewModel instance to ensure it's not null
        Console.WriteLine($"HomePage ViewModel: {viewModel}");
    }
}