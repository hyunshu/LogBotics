using FRC_App.Services;

namespace FRC_App;
public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private async void LogInUser(object sender, EventArgs e)
	{
		string username = UserNameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) { 
        	var user = await UserDatabase.GetUser(username);

			if (user != null && user.Password == password) {
				await DisplayAlert("Success", "Login successful!", "OK");
			} else {
				await DisplayAlert("Error", "Invalid username or password.", "OK");
			}

		} else {
			await DisplayAlert("Error", "Missing info.", "OK");
		}
	}

	private async void CreateAccount(object sender, EventArgs e)
	{
		string username = UserNameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
			try
            {
                await UserDatabase.AddUser(username, password);
                await DisplayAlert("Success", "Account created successfully!", "OK");
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

