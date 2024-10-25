
using FRC_App.Models;
using FRC_App.Services;
namespace FRC_App;

public partial class ForgotPasswordPage : ContentPage
{
	
	public ForgotPasswordPage()
	{
		InitializeComponent();
	}

	private async void GetUserSecurityQuestion(object sender, EventArgs e) {
		string username = UsernameEntry.Text;
		
		if (string.IsNullOrEmpty(username)) {
			await DisplayAlert("Error", "Missing username.", "OK");
			return;
		}

		var user = await UserDatabase.GetUser(username);
		if (user == null) {
			await DisplayAlert("Error", "User does not exist", "OK");
			return;
		}

		SecurityQuestionLabel.Text = user.SecurityQuestion;
		SecurityQuestionStack.IsVisible = true;
		SecurityAnswerStack.IsVisible = true;
		CheckAnswerButton.IsVisible = true;
	} 

	private async void CheckSecurityAnswer(object sender, EventArgs e) {
		string username = UsernameEntry.Text;
		var user = await UserDatabase.GetUser(username);

		string securityAnswer = SecurityAnswerEntry.Text;
		string realAnswer = user.SecurityAnswer;

		if (securityAnswer != realAnswer) {
			await DisplayAlert("Error", "Incorrect Answer.", "OK");
			return;
		} 

		ChangePasswordStack.IsVisible = true;
		SubmitPasswordStack.IsVisible = true;
	}

	private async void SaveNewPassword(object sender, EventArgs e) {
		string password = UserPasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

		if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) {
			await DisplayAlert("Error", "Password field(s) are empty.", "OK");
			return;
		}

		if (password.Length < 4) {
			await DisplayAlert("Error", "Password must be at least 4 characters long.", "OK");
			return;
		}

		if (password != confirmPassword) {
			await DisplayAlert("Error", "Passwords do not match.", "OK");
			return;
		}

		string username = UsernameEntry.Text;
		var user = await UserDatabase.GetUser(username);
		user.Password = password;
		await UserDatabase.updateUser(user);

		await DisplayAlert("Success", "Changed Password", "OK");
	}
}