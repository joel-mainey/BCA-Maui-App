namespace FirstMauiApp;

public partial class MainPage : ContentPage
{
	public MainPage(PlantsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

}

