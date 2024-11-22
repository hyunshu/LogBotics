using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

using FRC_App.Models;
using FRC_App.Services;
#if WINDOWS
    using Windows.Storage;
    using FRC_App.Platforms.Windows;
#endif
using System.Diagnostics;

namespace FRC_App;
public partial class ImportData : ContentPage
{
    public User currentUser { get; private set; }
    public string sessionName { get; private set; }
    public Session sessionData { get; private set; }
    public DataContainer dataContainer { get; private set; }
    public List<string> sessionsNames { get; private set; }

    public ImportData()
    {
        InitializeComponent();
        this.currentUser = UserSession.CurrentUser;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
        dataContainer = new DataContainer(currentUser);
        sessionsNames = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = null; // Force refresh
        DataSessionPicker.ItemsSource = sessionsNames;
	}

    private void OnDataSessionSelected(object sender, EventArgs e) {
        try {
            if (DataSessionPicker.SelectedIndex != -1)
            {
                sessionName = sessionsNames[DataSessionPicker.SelectedIndex];
                sessionData = dataContainer.getSession(sessionName);
            }
        } catch (Exception ex) {
            DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnImportButtonClicked(object sender, EventArgs e)
    {
        #if WINDOWS
        try
        {
            
            var filePaths = new List<string>{};
            var filePicker = new WindowsFilePicker();//DependencyService.Get<IFilePicker>();
            if (filePicker != null)
            {
                filePaths = await filePicker.PickFilesAsync();
                if (filePaths != null && filePaths.Any())
                {
                    Console.WriteLine();
                    foreach (var file in filePaths)
                    {
                        Console.WriteLine($"Selected file: {file}");
                    }
                } else {
                    throw new Exception("No CSV files selected! Couldn't import.");
                }
            }

            DataImport importDataStructure = new DataImport();
            List<List<List<double>>> recievedRawData = importDataStructure.FromCSV(filePaths);

             

            // Get the file name
            string sessionName = filePaths[0].Split("\\",StringSplitOptions.RemoveEmptyEntries).Last().Split("_",StringSplitOptions.RemoveEmptyEntries).First();
            string newName = await DisplayPromptAsync(
                "Rename Data Session",
                $"Enter a new name for this data session or leave as default:",
                initialValue: sessionName,
                placeholder: "New Session Name"
            );

            if (!string.IsNullOrEmpty(newName)) {
                sessionName = newName;
                Console.WriteLine("session name: " + sessionName);
            }
            // Display the selected file name
            SelectedFileLabel.Text = $"CSV files selected from Session: {sessionName}";
            
            DataContainer container = new DataContainer(currentUser);
            if (container.getSessionNames() != null && container.getSessionNames().Any()) {
                if (container.getSessionNames().Contains(sessionName)) {
                    throw new Exception($"A session named \"{sessionName}\" already exists under this user.");
                }
            }

            importDataStructure.sessionName = sessionName;
            await UserDatabase.storeData(currentUser,importDataStructure,recievedRawData);

            DataContainer updataedContainer = new DataContainer(currentUser);
            this.sessionData = updataedContainer.getSession(sessionName);
            this.sessionName = sessionName;
            updataedContainer.storeUpdates();


            Console.WriteLine($"Retrieved Data");
            Console.WriteLine($"Sessions:\n{currentUser.sessions}");
            Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
            Console.WriteLine($"{currentUser.dataUnits}");
            Console.WriteLine($"{currentUser.rawData}");


            await DisplayAlert("Success", "Data Imported", "Continue");
            
        }
        catch (Exception ex)
        {
            //Handle any exceptions that occur
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            SelectedFileLabel.Text = "No CSV files selected";
        }
        #endif
    }

    private async void ExportData(object sender, EventArgs e)
	{
        #if WINDOWS
        try {
		if (String.IsNullOrEmpty(currentUser.rawData)) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
			return;
		} 

		//All data prior to export:
		Console.WriteLine($"Sessions:\n{currentUser.sessions}");
		Console.WriteLine($"Old Stored Data:\n{currentUser.dataTypes}");
		Console.WriteLine($"{currentUser.dataUnits}");
		Console.WriteLine($"{currentUser.rawData}");

		if (this.sessionData is null) {
			await DisplayAlert("Error", "You have no session selected to Export.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser, this.sessionName); //Also reconstructs the dataStructure based on the retrieval

            var filePicker = new WindowsFilePicker(); 
            var directory = await filePicker.PickDirectoryAsync();
            if (string.IsNullOrEmpty(directory)) {
                throw new Exception("No or invalid export directory was selected. Export failed.");
            }

			DataExport export = new DataExport(exportDataStructure);
			export.ToCSV(retrievedRawData,this.sessionName, directory);

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
        } catch (Exception ex)
        {
            //Handle any exceptions that occur
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        #endif
	}

    private async void editDataDemo() {

		DataContainer dataContainer = new DataContainer(currentUser);
		List<string> sessions = dataContainer.getSessionNames();
		string sessionSelection = sessions.Last(); //Chosen from a dropdown (happen to choose last entry)
		Session session = dataContainer.getSession(sessionSelection);
		DataType removedType = session.DataTypes[0];
		session.DataTypes.RemoveAt(0); //Removes first DataType
		Console.WriteLine($"\nTesting Edit Data Functionality:\nRemoved DataType: \"{removedType.Name}\"" + 
				$" from Session: \"{session.Name}\". Warning! It may now be empty as a result.");

		dataContainer.storeUpdates();
	}

    private async void RunNetworkTablesClient(object sender, EventArgs e)
    {
        // Doesn't work on every machine as of 10/31/2024 - James Gilliam
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


            //Find and Read Text file:
            var filePath = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select the Live Robot Data .txt file import"
                });
                if (string.IsNullOrEmpty(filePath.FullPath)) {
                    throw new Exception("No or invalid .txt was selected. Export failed.");
                }

            DataImport dataStructure = new DataImport();
            List<List<List<double>>> rawData = dataStructure.FromRobot(filePath.FullPath); //Also appropriately reconstructions the dataStructure



            //TODO: Need popup to name the session of data imported from the Robot
            string newName = await DisplayPromptAsync(
                "Name New Data Session",
                $"Enter a new name for this data session:",
                initialValue: "",
                placeholder: "New Session Name"
            );
            dataStructure.sessionName = newName;

            await UserDatabase.storeData(currentUser,dataStructure,rawData);


            DataContainer dataContainer = new DataContainer(currentUser);
            dataContainer.storeUpdates(); // Just in case (may be unnecessary)


            Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
            Console.WriteLine($"{currentUser.dataUnits}");
            Console.WriteLine($"{currentUser.rawData}");

            await DisplayAlert("Success", "Data Recieved from Robot", "Continue");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}