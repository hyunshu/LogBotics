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
            // Open the file picker to select a file, allowing any file type
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Please select a file"
                // FileTypes = null will allow all file types
            });

            if (result != null)
            {
                // Get the file name
                var fileName = result.FileName;
                // Display the selected file name
                SelectedFileLabel.Text = $"Selected file: {fileName}";

                // Optional: Read the file stream if needed
                using (var stream = await result.OpenReadAsync())
                {
                    // You can process the file stream here
                }
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