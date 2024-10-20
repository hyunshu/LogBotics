
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
		SecurityQuestionLabel.IsVisible = true;
		SecurityAnswerEntry.IsVisible = true;
		CheckAnswer.IsVisible = true;
	} 

	private async void CheckSecurityAnswer(object sender, EventArgs e) {
		string username = UsernameEntry.Text;
		var user = await UserDatabase.GetUser(username);

		string securityAnswer = SecurityAnswerEntry.Text;
		string realAnswer = user.SecurityAnswer;

		if (securityAnswer != realAnswer) {
			await DisplayAlert("Error", "Incorrect Answer.", "OK");
		} else {
			await DisplayAlert("Success", "Correct Answer.", "OK");
		}

	}
}