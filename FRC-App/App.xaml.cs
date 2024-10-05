using FRC_App.Services;

namespace FRC_App;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		// Manually create an admin account when the app starts
		// You might want to check if the admin user already exists to avoid duplicates
		var adminUser = await UserDatabase.GetUser("admin");
		if (adminUser == null)
		{
			await UserDatabase.AddUser("admin", "qwer", true);
		}
	}

	// Other lifecycle methods
	protected override void OnSleep() { }
	protected override void OnResume() { }
}
