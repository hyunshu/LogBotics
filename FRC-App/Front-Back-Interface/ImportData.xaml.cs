using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;
public partial class ImportData : ContentPage
{
    public User currentUser { get; private set; }
    public string sessionName { get; private set; }
    public Session sessionData { get; private set; }

    public ImportData()
    {
        InitializeComponent();
        this.currentUser = UserSession.CurrentUser;
    }

    private async void OnImportButtonClicked(object sender, EventArgs e)
    {
        //try
        //{
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Please select a file to import"
            });
            
            string extension = result.FileName.Split(".",StringSplitOptions.RemoveEmptyEntries).Last();
            if (result != null && extension.Equals("csv"))
            {
                // Get the file name
                var fileName = result.FileName;
                string sessionName = fileName.Split("_",StringSplitOptions.RemoveEmptyEntries).First();
                // Display the selected file name
                SelectedFileLabel.Text = $"Selected file: {fileName}";

                DataImport importDataStructure = new DataImport();
                string directoryPath = result.FullPath.Substring(0, result.FullPath.Length - fileName.Length);
                string fileFamilyName = fileName.Split('_').First();
                List<List<List<double>>> recievedRawData = importDataStructure.FromCSV(directoryPath, fileFamilyName);

                await UserDatabase.storeData(currentUser,importDataStructure,recievedRawData);

                Console.WriteLine($"Sessions:\n{currentUser.sessions}");
                Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
                Console.WriteLine($"{currentUser.dataUnits}");
                Console.WriteLine($"{currentUser.rawData}");


                //Run Data Storage Test cases:
                // Generate Fake Test FRC Data to compare with retrieved stored data if no new data is imported this session:
                DataImport dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
                List<List<List<double>>> rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)

                Console.WriteLine($"Retrieved Data:\nRunning Test Cases . . .");
                int i = 0;
                foreach (string type in importDataStructure.dataTypes)
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
                foreach (List<string> file in importDataStructure.dataUnits)
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
                foreach (List<List<double>> file in recievedRawData)
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

                await DisplayAlert("Success", "Data Imported", "Continue");
            }
            else
            {
                // User canceled the file picking
                SelectedFileLabel.Text = "No CSV file selected";
            }

            
        //}
        //catch (Exception ex)
        //{
            // Handle any exceptions that occur
            //await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        //}
    }

    private async void ExportData(object sender, EventArgs e)
	{
		if (String.IsNullOrEmpty(currentUser.rawData)) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
			return;
		} 

		//Testing 11/12/24: (start)
		Console.WriteLine($"Sessions:\n{currentUser.sessions}");
		Console.WriteLine($"Old Stored Data:\n{currentUser.dataTypes}");
		Console.WriteLine($"{currentUser.dataUnits}");
		Console.WriteLine($"{currentUser.rawData}");

		// changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)

		editDataDemo();  //TESTING!!! Remove when edit data page is implemented 

		Console.WriteLine($"\nUpdated Sessions:\n{currentUser.sessions}");
		Console.WriteLine($"Edited Data Exported:\n{currentUser.dataTypes}");
		Console.WriteLine($"{currentUser.dataUnits}");
		Console.WriteLine($"{currentUser.rawData}");

		if (String.IsNullOrEmpty(currentUser.rawData)) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
			return;
		} 
		//Testing 11/12/24 (end)
		
		if (this.sessionData is null) {
			await DisplayAlert("Error", "You have no session selected to Export.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser, sessionName); //Also reconstructs the dataStructure based on the retrieval
			
			//Testing 10/31/2024:
			//exportDataStructure = new DataImport();
			//retrievedRawData = exportDataStructure.GenerateTestData();
			//Testing 10/31/2024

			DataExport export = new DataExport(exportDataStructure);
			export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
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
        DataImport dataStructure = new DataImport();
        string directoryPath = "../";
        string fileName = "RealRobotData.txt";
        List<List<List<double>>> rawData = dataStructure.FromRobot(directoryPath, fileName);



        //TODO: Need popup to name the session of data imported from the Robot
        string sessionName = "textboxEntry";
        dataStructure.sessionName = sessionName;



        await UserDatabase.storeData(currentUser,dataStructure,rawData);

        // changeSession();  // Remove this line when changeSession button is implemented and takes an (object sender, EventArgs e)

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
}