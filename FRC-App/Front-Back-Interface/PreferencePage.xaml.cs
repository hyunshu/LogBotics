using System.ComponentModel;
using Microsoft.Maui.Storage; // for Preferences

using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;
public partial class PreferencePage : ContentPage, INotifyPropertyChanged
{
    public User currentUser { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public PreferencePage(User user)
    {
        InitializeComponent();
        currentUser = user;

        // Set BindingContext to the global settings view model
        BindingContext = App.GlobalSettings;

        LoadUserThemePreference();
    }

    // Load the user's saved theme preference if it exists
    private void LoadUserThemePreference()
    {
        string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";
        string savedTheme = Preferences.Get(userKey, "Light Theme"); // Default to "Light Theme"
        ((App)Application.Current).LoadTheme(savedTheme); // Apply the saved theme
    }

    private void OnFontColorChanged(object sender, EventArgs e)
    {
        if (colorPicker.SelectedItem is string selectedColor)
        {
            Color color;

            // Check if "Default" is selected and apply theme-based color
            if (selectedColor == "Default")
            {
                // Set color based on current theme
                var currentTheme = Application.Current.RequestedTheme;
                color = currentTheme == AppTheme.Dark ? Colors.White : Colors.Black;
            }
            else
            {
                // Map other selected colors to Color objects
                color = selectedColor switch
                {
                    "Black" => Colors.Black,
                    "Red" => Colors.Red,
                    "Green" => Colors.Green,
                    "Blue" => Colors.Blue,
                    "Orange" => Colors.Orange,
                    "Purple" => Colors.Purple,
                    "White" => Colors.White,
                    _ => Colors.Black // Fallback
                };
            }

            // Set the FontColor in GlobalSettings
            App.GlobalSettings.FontColor = color;
        }
    }

    private void OnFontTypeChanged(object sender, EventArgs e)
    {
        if (fontTypePicker.SelectedItem is string selectedFontType)
        {
            App.GlobalSettings.FontType = selectedFontType;
        }
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