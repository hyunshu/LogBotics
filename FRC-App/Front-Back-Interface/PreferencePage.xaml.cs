using System.ComponentModel;
using Microsoft.Maui.Storage; // for Preferences

using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;
public partial class PreferencePage : ContentPage, INotifyPropertyChanged
{
    public User currentUser  {get; private set; }

    private double _fontSize;

    public event PropertyChangedEventHandler PropertyChanged;

    public double FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize != value)
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
                SaveFontSizePreference(); // Save the font size whenever it changes
            }
        }
    }
    
    public PreferencePage(User user)
    {
        InitializeComponent();
        currentUser = user;
        BindingContext = this; // Set the BindingContext to the current page
        LoadFontSizePreference(); // Load the font size preference on page load
    }

    // Save the font size to Preferences
    private void SaveFontSizePreference()
    {
        Preferences.Set("UserFontSize", _fontSize);
    }

    // Load the font size from Preferences
    private void LoadFontSizePreference()
    {
        FontSize = Preferences.Get("UserFontSize", 18); // Default to 18 if no preference is saved
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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