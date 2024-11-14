using FRC_App.Models;
using FRC_App.Services;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
//using Microsoft.Maui.Controls.Compatibility.Platform.iOS; //Causes and error on windows

namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }
	public Session sessionData { get; private set; }

	public HomePage()
	{
		InitializeComponent();
		currentUser = UserSession.CurrentUser;
		BindingContext = currentUser;

		if (currentUser.sessions != null) {
			//changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)
		}

		loadUserPreferences();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		BindingContext = null;
		BindingContext = currentUser;
	}

	
	//Demo for front-end devs (needs to be implemented as a button):
	// private async void changeSession() {
	// 	bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

	// 	if (!hasData) {
	// 		await DisplayAlert("Error", "You have no data to display.", "OK");
	// 		return;
	// 	} 

	// 	//TODO: Needs dropdown implementation
	// 	DataContainer dataContainer = new DataContainer(currentUser);
	// 	List<string> sessions = dataContainer.getSessionNames();
	// 	string sessionSelection = sessions.Last(); //Chosen from a dropdown

	// 	this.sessionData = dataContainer.getSession(sessionSelection);
	// }

	//Demo for front-end devs (just demonstrates how to propate data updates for HunShu's edit data page):
	//Remove this function when edit data page is implemented
	// private async void editDataDemo() {

	// 	DataContainer dataContainer = new DataContainer(currentUser);
	// 	List<string> sessions = dataContainer.getSessionNames();
	// 	string sessionSelection = sessions.Last(); //Chosen from a dropdown (happen to choose last entry)
	// 	Session session = dataContainer.getSession(sessionSelection);
	// 	DataType removedType = session.DataTypes[0];
	// 	session.DataTypes.RemoveAt(0); //Removes first DataType
	// 	Console.WriteLine($"\nTesting Edit Data Functionality:\nRemoved DataType: \"{removedType.Name}\"" + 
	// 			$" from Session: \"{session.Name}\". Warning! It may now be empty as a result.");

	// 	dataContainer.storeUpdates();
	// }

	// private async void ImportData(object sender, EventArgs e)
	// {
	// 	await Shell.Current.GoToAsync("///importdata");
	// }

	// private async void ExportData(object sender, EventArgs e)
	// {
	// 	if (String.IsNullOrEmpty(currentUser.rawData)) {
	// 		await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
	// 		return;
	// 	} 

	// 	//Testing 11/12/24: (start)
	// 	Console.WriteLine($"Sessions:\n{currentUser.sessions}");
	// 	Console.WriteLine($"Old Stored Data:\n{currentUser.dataTypes}");
	// 	Console.WriteLine($"{currentUser.dataUnits}");
	// 	Console.WriteLine($"{currentUser.rawData}");

	// 	// changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)

	// 	editDataDemo();  //TESTING!!! Remove when edit data page is implemented 

	// 	Console.WriteLine($"\nUpdated Sessions:\n{currentUser.sessions}");
	// 	Console.WriteLine($"Edited Data Exported:\n{currentUser.dataTypes}");
	// 	Console.WriteLine($"{currentUser.dataUnits}");
	// 	Console.WriteLine($"{currentUser.rawData}");

	// 	if (String.IsNullOrEmpty(currentUser.rawData)) {
	// 		await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
	// 		return;
	// 	} 
	// 	//Testing 11/12/24 (end)
		
	// 	if (this.sessionData is null) {
	// 		await DisplayAlert("Error", "You have no session selected to Export.", "OK");
	// 	} else {
	// 		DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
	// 		List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser); //Also reconstructs the dataStructure based on the retrieval
			
	// 		//Testing 10/31/2024:
	// 		//exportDataStructure = new DataImport();
	// 		//retrievedRawData = exportDataStructure.GenerateTestData();
	// 		//Testing 10/31/2024

	// 		DataExport export = new DataExport(exportDataStructure);
	// 		export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

	// 		await DisplayAlert("Success", "Data Exported", "Continue"); 
	// 	}
	// }

	// private async void Preference(object sender, EventArgs e)
	// {
	// 	await Shell.Current.GoToAsync("///preferencepage");
	// }


	public void loadUserPreferences() {
		string userThemeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";

		string userTheme = Preferences.Get(userThemeKey, "Dark Theme");
		((App)Application.Current).LoadTheme(userTheme);

		// UpdateChartColors();
	}

	private async void LogOut(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
		if (answer)
		{
			((App)Application.Current).LoadTheme("Dark Theme");
			await Shell.Current.GoToAsync("///loginpage");	
		}
	}


/*	private async void LaunchNetworkTablesClient(object sender, EventArgs e)
{
    try
    {
        await Task.Run(() =>
        {
            Console.WriteLine("Attempting to launch NetworkTables client...");
            
            var processInfo = new ProcessStartInfo
            {
                FileName = @"C:\tools\dart-sdk\bin\dart.exe",
                Arguments = "run ./networkTablesClient.dart",
                WorkingDirectory = @"C:\Users\Jenna\Documents\GitHub\LogBotics\networkTables",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            if (process != null)
            {
                using (var outputReader = process.StandardOutput)
                {
                    string result = outputReader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using (var errorReader = process.StandardError)
                {
                    string errorResult = errorReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorResult))
                    {
                        Console.WriteLine($"Error: {errorResult}");
                    }
                }

                process.WaitForExit();
            }
            else
            {
                Console.WriteLine("Failed to start the process.");
            }
        });
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to launch NetworkTables client: {ex.Message}", "OK");
    }
} 
*/

// private async void RunNetworkTablesClient(object sender, EventArgs e)
// {
	// DataImport dataStructure = new DataImport();
	// string directoryPath = "../";
	// string fileName = "RealRobotData.txt";
	// List<List<List<double>>> rawData = dataStructure.FromRobot(directoryPath, fileName);



	// //TODO: Need popup to name the session of data imported from the Robot
	// string sessionName = "textboxEntry";
	// dataStructure.sessionName = sessionName;



	// await UserDatabase.storeData(currentUser,dataStructure,rawData);

	// // changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)

	// Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
	// Console.WriteLine($"{currentUser.dataUnits}");
	// Console.WriteLine($"{currentUser.rawData}");

	// await DisplayAlert("Success", "Data Recieved from Robot", "Continue");

	/* Doesn't work on every machine as of 10/31/2024 - James Gilliam
    try
    {
        string dartExePath = @"C:\tools\dart-sdk\bin\dart.exe"; // Updated Dart executable path
        string scriptPath = @"..\networkTables\networkTablesClientToFile.dart";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = dartExePath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            Dispatcher.Dispatch(() =>
            {
                if (!string.IsNullOrEmpty(output))
                {
                    OutputLabel.Text = $"Output: {output}";
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    OutputLabel.Text = $"Error: {error}";
                }
                else
                {
                    OutputLabel.Text = "NetworkTables Client executed with no output.";
                }
            });

            process.WaitForExit();
        }

        await DisplayAlert("Success", "NetworkTables Client script executed successfully.", "OK");
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
	*/
	//}

// private async void OpenMapPage(object sender, EventArgs e)
// {
// 	if (currentUser.rawData is null) {
// 		await DisplayAlert("Error", "You have no data to display.", "OK");
// 		return;
// 	} else if (this.sessionData is null) {
// 		await DisplayAlert("Error", "You have no session selected.", "OK");
// 		return;
// 	} else {
//     await Navigation.PushAsync(new MapPage(currentUser,this.sessionData));
// 	}
// }

// private async void OnDebuggingButtonClicked(object sender, EventArgs e)
// {
//     await Shell.Current.GoToAsync("///debuggingpage");
// }

// private async void OnReportBugButtonClicked(object sender, EventArgs e)
// {
//     await Shell.Current.GoToAsync("///reportbugpage");
// }

}