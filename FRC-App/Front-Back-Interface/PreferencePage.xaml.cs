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

           
            
        }
    }
}