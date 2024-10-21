using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            

            if (result != null)
            {
                await DisplayAlert("Success", "Data Imported", "Continue");
                // Get the file name
                var fileName = result.FileName;
                // Display the selected file name
                SelectedFileLabel.Text = $"Selected file: {fileName}";

                DataImport dataStructure = new DataImport();
                string directoryPath = result.FullPath.Substring(0, result.FullPath.Length - fileName.Length);
                string fileFamilyName = fileName.Split('_').First();
                List<List<List<double>>> rawData = dataStructure.FromCSV(directoryPath, fileFamilyName);
                await UserDatabase.storeData(currentUser,dataStructure,rawData);

                Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
                Console.WriteLine($"{currentUser.dataUnits}");
                Console.WriteLine($"{currentUser.rawData}");



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
            }
            else
            {
                // User canceled the file picking
                SelectedFileLabel.Text = "No file selected";
            }

            
        //}
        //catch (Exception ex)
        //{
            // Handle any exceptions that occur
            //await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        //}
    }
}