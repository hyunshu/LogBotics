namespace FRC_App;
public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private void LogInUser(object sender, EventArgs e)
	{
		LoginBtn.Text = "Logging in...";
		SemanticScreenReader.Announce(LoginBtn.Text);

		string username = UserNameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) { 
        	

		} else {
			LoginBtn.Text = "Missing info!";
			SemanticScreenReader.Announce(LoginBtn.Text);
		}
	}

	private void CreateAccount(object sender, EventArgs e)
	{
		CreateAcctBtn.Text = "Creating account...";
		SemanticScreenReader.Announce(CreateAcctBtn.Text);

		string username = UserNameEntry.Text;
		string password = UserPasswordEntry.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
		

		} else {
			LoginBtn.Text = "Missing info!";
			SemanticScreenReader.Announce(LoginBtn.Text);
		}
	}
}

