using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;
public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private void OnTogglePasswordVisibilityToggled(object sender, ToggledEventArgs e)
	{
		UserPasswordEntry.IsPassword = !e.Value;
	}

	private async void LogInUser(object sender, EventArgs e)
	{
		string username = UsernameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) { 
			var user = await UserDatabase.GetUser(username);

			if (user != null && user.Password == password) {
				if (await UserDatabase.checkActive(username)) {
					await DisplayAlert("Error", "User is already online on a different device.", "OK");
				} else if (user.IsAdmin) {
					await DisplayAlert("Success", "Admin login successful!", "Get Started");
					await UserDatabase.login(user);
					// Application.Current.MainPage = new NavigationPage(new HomePage(user));  // Redirect to admin page
				} else {
					await DisplayAlert("Success", "Login successful!", "Get Started");
					await UserDatabase.login(user);
					UserSession.CurrentUser = user;
					Application.Current.MainPage = new AppShell();
					await Shell.Current.GoToAsync("///homepage");
				}
			} else {
				await DisplayAlert("Error", "Invalid username or password.", "OK");
			}
		} else {
			await DisplayAlert("Error", "Missing info.", "OK");
		}
	}


	private async void RedirectToCreateAccountPage(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CreateAccountPage());
	}

	private async void RedirectToForgotPasswordPage(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ForgotPasswordPage());
	}
}

