using FRC_App.Services;

namespace FRC_App;
public partial class LoginPage : ContentPage
{

	public LoginPage()
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
				if (user.IsAdmin) {
					await DisplayAlert("Success", "Admin login successful!", "Get Started");
					await Navigation.PushAsync(new HomePage());  // Redirect to admin page
				} else {
					await DisplayAlert("Success", "Login successful!", "Get Started");
					await Navigation.PushAsync(new HomePage());  // Redirect to regular homepage
				}
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

