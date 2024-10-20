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
		// Manually create an admin account to the data base when the app starts
        // This is for testing purposes only
        // This should be removed when the app is deployed
        
        // Deleted admin account due to bugs
	}

	// Apply theme
    public void LoadTheme(string theme)
    {
        if (theme == "Light Theme")
        {
            Current.UserAppTheme = AppTheme.Light;
        }
        else
        {
            Current.UserAppTheme = AppTheme.Dark;
        }
    }

    // Apply font size
    public void SetAppFontSize(string fontSize)
    {
        double size = fontSize switch
        {
            "Small" => 12,
            "Medium" => 16,
            "Large" => 20,
            _ => 16,
        };
        Resources["AppFontSize"] = size;
    }

    // Apply layout style
    public void SetAppLayoutStyle(string layoutStyle)
    {
		//impleme nt layout style
    }

    // Save and load preferences - This is not working properly
	// Have to implement this such that each user has their own preferences saved
	
	// NOTE: Implemented these in PreferencePage and HomePage

    // public void SaveThemePreference(string theme) => Preferences.Set("theme", theme);
    // public string GetSavedTheme() => Preferences.Get("theme", "Light Theme");

    // public void SaveFontSizePreference(string fontSize) => Preferences.Set("fontSize", fontSize);
    // public string GetSavedFontSize() => Preferences.Get("fontSize", "Medium");

    // public void SaveLayoutPreference(string layoutStyle) => Preferences.Set("layout", layoutStyle);
    // public string GetSavedLayoutStyle() => Preferences.Get("layout", "Spacious");

    public void SaveNotificationPreference(bool isEnabled) => Preferences.Set("notificationsEnabled", isEnabled);
    public bool GetNotificationPreference() => Preferences.Get("notificationsEnabled", true);

    public void LogOutUser()
    {
        // Perform any cleanup or session management needed during log out.
    }

	// Other lifecycle methods
	protected override void OnSleep() { }
	protected override void OnResume() { }
}
