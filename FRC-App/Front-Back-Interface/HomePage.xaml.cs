using FRC_App.Models;
using FRC_App.Services;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
//using Microsoft.Maui.Controls.Compatibility.Platform.iOS; //Causes and error on windows

namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }
	public Session sessionData { get; private set; }

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;
		BindingContext = currentUser;

		if (user.rawData != null) {
			changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)
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
	private async void changeSession() {
		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (!hasData) {
			await DisplayAlert("Error", "You have no data to display.", "OK");
			return;
		} 

		//TODO: Needs dropdown implementation
		DataContainer dataContainer = new DataContainer(currentUser);
		List<string> sessions = dataContainer.getSessionNames();
		string sessionSelection = sessions[0]; //Chosen from a dropdown

		this.sessionData = dataContainer.getSession(sessionSelection);
	}

	private async void AddPlot(object sender, EventArgs e) {
		await Navigation.PushAsync(new AddPlotPage(currentUser));
	}

	private async void ImportData(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ImportData(currentUser));
	}

	private async void ExportData(object sender, EventArgs e)
	{
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
			return;
		} 

		changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)
		
		if (this.sessionData is null) {
			await DisplayAlert("Error", "You have no session selected to Export.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser,sessionData.Name); //Also reconstructs the dataStructure based on the retrieval
			
			//Testing 10/31/2024:
			//exportDataStructure = new DataImport();
			//retrievedRawData = exportDataStructure.GenerateTestData();
			//Testing 10/31/2024

			DataExport export = new DataExport(exportDataStructure);
			export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
	}

	private async void Preference(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new PreferencePage(currentUser));
	}

	private async void Instruction(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new InstructionPage(currentUser));
	}

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
			Application.Current.MainPage = new NavigationPage(new LoginPage());	
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

private async void RunNetworkTablesClient(object sender, EventArgs e)
{
	DataImport dataStructure = new DataImport();
	string directoryPath = "../";
	string fileName = "RealRobotData.txt";
	List<List<List<double>>> rawData = dataStructure.FromRobot(directoryPath, fileName);



	//TODO: Need popup to name the session of data imported from the Robot
	string sessionName = "textboxEntry";



	await UserDatabase.storeData(currentUser,dataStructure,rawData,sessionName);

	changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)

	Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
	Console.WriteLine($"{currentUser.dataUnits}");
	Console.WriteLine($"{currentUser.rawData}");

	await DisplayAlert("Success", "Data Recieved from Robot", "Continue");

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
}

private async void OpenMapPage(object sender, EventArgs e)
{
	if (currentUser.rawData is null) {
		await DisplayAlert("Error", "You have no data to display.", "OK");
		return;
	} else if (this.sessionData is null) {
		await DisplayAlert("Error", "You have no session selected.", "OK");
		return;
	} else {
    await Navigation.PushAsync(new MapPage(currentUser,this.sessionData));
	}
}

private async void OnDebuggingButtonClicked(object sender, EventArgs e)
{
    await Navigation.PushAsync(new DebuggingPage());
}

private async void OnReportBugButtonClicked(object sender, EventArgs e)
{
    await Navigation.PushAsync(new ReportBugPage());
}


	
}