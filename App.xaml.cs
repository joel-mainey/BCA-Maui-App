using FirstMauiApp.View;

namespace FirstMauiApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new AppShell();

        //MainPage = new NavigationPage(new HomePage());
	}
}
