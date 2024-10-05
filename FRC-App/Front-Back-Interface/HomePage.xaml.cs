using FRC_App.Models;


namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;
		BindingContext = currentUser;
	}

	DataImport dataStructure;
	public List<List<List<double>>> rawData;

	private async void ImportData(object sender, EventArgs e)
	{
		// Generate Fake Test FRC Data:
        dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
        rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)

		Console.WriteLine($"Logged in user: {currentUser.Username}");		// testing if user data was passed from loginPage
		await DisplayAlert("Success", "Data Imported", "Continue"); 
	}

	private async void ExportData(object sender, EventArgs e)
	{
		if (dataStructure is null) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
		} else {
			DataExport export = new DataExport(dataStructure);
			export.ToCSV(rawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
	}

	private async void LogOut(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
		if (answer)
		{
			Application.Current.MainPage = new NavigationPage(new LoginPage());
		}
	}
}