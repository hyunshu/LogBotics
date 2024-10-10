using FRC_App.Models;
using FRC_App.Services;


namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;

		Console.WriteLine($"Logged in user: {currentUser.Username}");  // testing if user data was passed from loginPage

		BindingContext = currentUser;

		loadUserPreferences();
	}


	DataImport dataStructure;
	public List<List<List<double>>> rawData;

	private async void ImportData(object sender, EventArgs e)
	{
		// Generate Fake Test FRC Data:
        dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
        rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)

		await UserDatabase.storeData(currentUser,dataStructure,rawData);

		Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
		Console.WriteLine($"{currentUser.dataUnits}");
		Console.WriteLine($"{currentUser.rawData}");

		await DisplayAlert("Success", "Data Imported", "Continue"); 
	}

	private async void ExportData(object sender, EventArgs e)
	{
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser); //Also reconstructs the dataStructure based on the retrieval


			//Run Data Storage Test cases:
			if (dataStructure == null)
			{
				// Generate Fake Test FRC Data to compare with retrieved stored data if no new data is imported this session:
        		dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
        		rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)
			}

			Console.WriteLine($"Retrieved Data:\nRunning Test Cases . . .");
			int i = 0;
			foreach (string type in exportDataStructure.dataTypes)
			{
				if (!type.Equals(dataStructure.dataTypes[i]))
				{
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Types Storage Failure!");
				} else {
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Types Storage Passed.");
				}
				i++;
			}

			i = 0;
			foreach (List<string> file in exportDataStructure.dataUnits)
			{
				int j = 0;
				int errors = 0;
				foreach (string unit in file) 
				{
					if (!unit.Equals(dataStructure.dataUnits[i][j]))
					{
						Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Units Storage Failure!\nColumn: {j+1}");
						errors++;
					}
					j++;
				}
				if (errors == 0)
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Units Storage Passed.");
				i++;
			}

			i = 0;
			foreach (List<List<double>> file in retrievedRawData)
			{
				int j = 0;
				int errors = 0;
				foreach (List<double> column in file) 
				{
					int k = 0;
					foreach (double x in column) 
					{
						if (x != rawData[i][j][k]) 
						{
							Console.WriteLine($"{dataStructure.dataTypes[i]}: Raw Data Storage Failure!\nColumn: {j+1}\nEntry: {k+1}");
							errors++;
						}
						k++;
					}
					j++;
				}
				if (errors == 0)
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Raw Data Storage Passed.");
				i++;
			}


			DataExport export = new DataExport(dataStructure);
			export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
	}

	private async void Preference(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new PreferencePage(currentUser));
	}

	public void loadUserPreferences() {
		string userThemeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";
		string userFontSizeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_fontSize";
		string userLayoutKey = $"{currentUser.Username}_{currentUser.TeamNumber}_layoutStyle";

		string userTheme = Preferences.Get(userThemeKey, "Dark Theme");
		((App)Application.Current).LoadTheme(userTheme);

		string userFontSize = Preferences.Get(userFontSizeKey, "Medium");
		((App)Application.Current).SetAppFontSize(userFontSize);

		string userLayout = Preferences.Get(userLayoutKey, "Spacious");
		((App)Application.Current).SetAppLayoutStyle(userLayout);
		
	}

	private async void LogOut(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
		if (answer)
		{
			Application.Current.MainPage = new NavigationPage(new LoginPage());
			// ((App)Application.Current).LoadTheme("Dark Theme");
		}
	}
}