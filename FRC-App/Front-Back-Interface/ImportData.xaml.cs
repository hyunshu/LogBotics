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
        try
        {
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

                // Read the file stream
                //using (var stream = await result.OpenReadAsync())
                //{
                    // Read the file
                    DataImport dataStructure = new DataImport();
                    string directoryPath = result.FullPath.Substring(0, result.FullPath.Length - fileName.Length);
                    string fileFamilyName = fileName.Split('_').First();
                    List<List<List<double>>> rawData = dataStructure.FromCSV(directoryPath, fileFamilyName);
                    dataStructure.StoreRawData(rawData, currentUser);

                    Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
		            Console.WriteLine($"{currentUser.dataUnits}");
		            Console.WriteLine($"{currentUser.rawData}");
                //}
            }
            else
            {
                // User canceled the file picking
                SelectedFileLabel.Text = "No file selected";
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}