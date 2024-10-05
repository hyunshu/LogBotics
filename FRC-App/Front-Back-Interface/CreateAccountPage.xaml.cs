using FRC_App.Services;

namespace FRC_App;
public partial class CreateAccountPage : ContentPage
{
    public CreateAccountPage()
    {
        InitializeComponent();
    }

    private async void RedirectToCreateAccountPage(object sender, EventArgs e)
	{
        string teamName = TeamNameEntry.Text;
        string teamNumber = TeamNumberEntry.Text;
		string username = UsernameEntry.Text;
		string password = UserPasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
			try
            {
                await UserDatabase.AddUser(username, password);
                await DisplayAlert("Success", "Account created successfully!", "Log in");
				await Navigation.PushAsync(new HomePage());
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