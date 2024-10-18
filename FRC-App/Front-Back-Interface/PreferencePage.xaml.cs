using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;
public partial class PreferencePage : ContentPage
{
    public User currentUser  {get; private set; }

    public PreferencePage(User user)
    {
        InitializeComponent();
        currentUser = user;
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if (themePicker.SelectedItem is string selectedTheme)
        {
            // Change the app theme based on user selection
            ((App)Application.Current).LoadTheme(selectedTheme);
            DisplayAlert("Theme Changed", $"You have selected the {selectedTheme}.", "OK"); 

            // Save the user's theme to their preferences
            string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";
            Preferences.Set(userKey, selectedTheme);
        }
    }

    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (fontSizePicker.SelectedItem is string selectedFontSize)
        {
            // Apply the font size throughout the app
            ((App)Application.Current).SetAppFontSize(selectedFontSize);

            DisplayAlert("Font Size Changed", $"You have selected {selectedFontSize} font size.", "OK");

            // Save the font size preference to the user 
            string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_fontSize";
            Preferences.Set(userKey, selectedFontSize);
        }
    }

    private void OnLayoutChanged(object sender, EventArgs e)
    {
        if (layoutPicker.SelectedItem is string selectedLayout)
        {
            // Apply layout changes (e.g., compact or spacious)
            ((App)Application.Current).SetAppLayoutStyle(selectedLayout);

            DisplayAlert("Layout Changed", $"You have selected the {selectedLayout} layout style.", "OK");

            // Save the layout preference to the user
            string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_layoutStyle";
            Preferences.Set(userKey, selectedLayout);;
        }
    }

    private async void OnEditAccountClicked(object sender, EventArgs e)
    {
        // Navigate to the account settings page or open an editor for user account details
        /* Skeleton code for navigating to account settings page
        await Navigation.PushAsync(new AccountSettingsPage(currentUser));
        */
        await Navigation.PushAsync(new EditAccountInfoPage(currentUser));
    }

    private async void OnLogOutClicked(object sender, EventArgs e)
    {
        DisplayAlert("YOU NEED TO IMLEMENT THIS FUNCTIONALITY LOL", "This feature is not yet implemented.", "OK");
        /* Skeleton code for logging out a user
        bool confirm = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
        if (confirm)
        {
            // Log out the user, possibly clear session data
            ((App)Application.Current).LogOutUser();

            // Navigate back to login page or main page
            await Navigation.PopToRootAsync();
        }
        */
    }
}