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

		if (!string.IsNullOrEmpty(teamName) && !string.IsNullOrEmpty(teamNumber)
                && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
                && !string.IsNullOrEmpty(confirmPassword)) {

            bool userExists = await UserDatabase.CheckUserExistsAsync(username);
            if (userExists)
            {
                await DisplayAlert("Error", "Username already exists. Please choose another username.", "OK");
                return;
            }

            if (password.Length < 4) {
                await DisplayAlert("Error", "Password must be at least 4 characters long.", "OK");
                return;
            }

            if (!int.TryParse(teamNumber, out int teamNum) || teamNum < 1 || teamNum > 99) {
                await DisplayAlert("Error", "Team number must be between 1 and 99.", "OK");
                return;
            }

            if (password != confirmPassword) {
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

		} else {
			await DisplayAlert("Error", "Missing info.", "OK");
		}
	}
}