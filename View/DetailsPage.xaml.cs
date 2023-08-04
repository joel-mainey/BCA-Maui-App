namespace FirstMauiApp.View;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(PlantsDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}