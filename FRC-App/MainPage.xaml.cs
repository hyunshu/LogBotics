namespace FRC_App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnSubmitClicked(object sender, EventArgs e)
	{
		LoginBtn.Text = "Logging in...";
		SemanticScreenReader.Announce(LoginBtn.Text);

		string username = UserNameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
			Navigation.PushAsync(new MainPage());
		} else {
			LoginBtn.Text = "Missing info!";
			SemanticScreenReader.Announce(LoginBtn.Text);
		}
	}
}

