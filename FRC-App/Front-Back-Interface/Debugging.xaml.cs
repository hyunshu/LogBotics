using Microsoft.Maui.Storage;
using System.Text;
using System.Text.RegularExpressions;

namespace FRC_App
{
    public partial class DebuggingPage : ContentPage
    {
        public DebuggingPage()
        {
            InitializeComponent();
        }

        // Event handler for file upload button
private async void OnUploadFileClicked(object sender, EventArgs e)
{
    try
    {
        // Create a custom file type for .dsevents files or text files
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "public.text" } },
            { DevicePlatform.Android, new[] { "text/plain" } },
            { DevicePlatform.WinUI, new[] { ".dsevents", ".txt" } },
            { DevicePlatform.macOS, new[] { "public.text" } },
        });

        // Pick the .dsevents file or a text file
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select a .dsevents or text file",
            FileTypes = customFileType
        });

        if (result != null)
        {
            // Read the content of the selected file
            string fileContent = await File.ReadAllTextAsync(result.FullPath);

            // Parse the file content for errors and warnings
            var parsedOutput = ParseDSEventsFile(fileContent);

            // Display the parsing status and output
            ParsingStatusLabel.Text = "Parsing complete. Results:";
            ParsedOutputLabel.Text = parsedOutput;
        }
        else
        {
            ParsingStatusLabel.Text = "File selection was canceled.";
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error occurred while uploading the file: {ex.Message}", "OK");
    }
}


        // Method to parse the .dsevents file for errors and warnings
        private string ParseDSEventsFile(string fileContent)
        {
            var output = new StringBuilder();

            // Define regex patterns for errors and warnings
            string errorPattern = @"(?i)(error|failed|lost communication)";
            string warningPattern = @"(?i)(warning)";

            // Split the content into lines
            string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, errorPattern))
                {
                    output.AppendLine("Error: " + line);
                }
                else if (Regex.IsMatch(line, warningPattern))
                {
                    output.AppendLine("Warning: " + line);
                }
            }

            return output.Length > 0 ? output.ToString() : "No errors or warnings found.";
        }
    }
}
