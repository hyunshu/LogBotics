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

    public ImportData(User user)
    {
        InitializeComponent();
        currentUser = user;
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
                // Display the selected file name
                SelectedFileLabel.Text = $"Selected file: {fileName}";

                DataImport importDataStructure = new DataImport();
                string directoryPath = result.FullPath.Substring(0, result.FullPath.Length - fileName.Length);
                string fileFamilyName = fileName.Split('_').First();
                List<List<List<double>>> recievedRawData = importDataStructure.FromCSV(directoryPath, fileFamilyName);
                await UserDatabase.storeData(currentUser,importDataStructure,recievedRawData);

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



                ////Testing 10/16/2024 Begin (also a demo for front-end devs):
                DataContainer dataContainer = new DataContainer(currentUser);

                //Determine the dataType:
                List<string> dataTypeNames = dataContainer.getDataTypeNames();  // Display these in first set of buttons
                string typeSelection = dataTypeNames[0];  // This would be from the first set of buttons
                DataType targetType = dataContainer.getDataType(typeSelection);

                //Determine the dataColumn:
                List<string> columnLabels = targetType.getColumnLabels();  // Display these in second set of buttons
                string columnSelectionx = columnLabels[0];  // This would be from the second set of buttons
                string columnSelectiony = columnLabels[1];  // This would be from the second set of buttons
                Column targetColumnX = targetType.getColumn(columnSelectionx);  //ie. x
                Column targetColumnY = targetType.getColumn(columnSelectiony);  //ie. y

                //Axis Label and Data ready for ploting:
                string axisLabel = targetColumnX.Label; // Or the columnSelection string
                List<double> axisData = targetColumnX.Data;

                Plot testPlot = new Plot(targetColumnX, targetColumnY);
                ////Testing 10/16/2024 End:
                


                ///Testing 10/25/2024 Begin for Mapping (also a demo for front-end devs):
                DataContainer dataContainerM = new DataContainer(currentUser);

                //Determine the dataType for mapping:
                List<string> dataTypeNamesM = dataContainerM.getDataTypeNames();  // Display these in first set of buttons
                string typeSelectionM = dataTypeNamesM[1];  // This would be from the first set of buttons
                DataType targetTypeM = dataContainer.getDataType(typeSelectionM);

                //Determine the dataColumn:
                List<string> columnLabelsM = targetTypeM.getColumnLabels();  // Display these in second set of buttons
                string columnSelectionTime = columnLabelsM[0];  // This would be from the second set of buttons
                string columnSelectionxAccel = columnLabelsM[1];  // This would be from the second set of buttons
                string columnSelectionyAccel = columnLabelsM[2];  // This would be from the second set of buttons
                Column targetColumnTime = targetTypeM.getColumn(columnSelectionTime);  //ie. time
                Column targetColumnXAccel = targetTypeM.getColumn(columnSelectionxAccel);  //ie. x acceleration
                Column targetColumnYAccel = targetTypeM.getColumn(columnSelectionyAccel);  //ie. y acceleration

                Map testMap = new Map(targetColumnTime,targetColumnXAccel,targetColumnYAccel);
                SKPath testPath = testMap.GeneratePath();
                SKBitmap testGrid = testMap.GenerateGrid();

                SKCanvas mapFigure = new SKCanvas(testGrid);
                var paint1 = new SKPaint {
                    TextSize = 64.0f,
                    IsAntialias = true,
                    Color = new SKColor(255, 0, 0),
                    Style = SKPaintStyle.Fill
                };
                //mapFigure.DrawPoints(SKPointMode.Points,testPath,paint1); //Not sure how to set up .xaml to test this
                ////Testing 10/25/2024 End:

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
}