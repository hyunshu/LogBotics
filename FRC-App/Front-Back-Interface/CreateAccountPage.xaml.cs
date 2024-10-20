using FRC_App.Services;

namespace FRC_App;
public partial class CreateAccountPage : ContentPage
{
    public CreateAccountPage()
    {
        InitializeComponent();
    }

    private async void CreateAccount(object sender, EventArgs e)
	{
        string teamName = TeamNameEntry.Text;
        string teamNumber = TeamNumberEntry.Text;
		string username = UsernameEntry.Text;
		string password = UserPasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;



        // Validate Empty Inputs
        if (string.IsNullOrEmpty(teamName) || string.IsNullOrEmpty(teamNumber)
            || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)
            || string.IsNullOrEmpty(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill out all fields.", "OK");
            return;
        }
        // Validate confirm password
        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            await UserDatabase.AddUser(teamName, teamNumber, username, password, false);
            await DisplayAlert("Success", "Account created successfully!", "Go Back to Login");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            // Handle case where user already exists or any other error
            await DisplayAlert("Error", ex.Message, "OK");
        }
	}
}