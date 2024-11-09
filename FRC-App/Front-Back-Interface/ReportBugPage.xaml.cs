using Microsoft.Maui.Controls;
using System.Net.Mail;

namespace FRC_App
{
    public partial class ReportBugPage : ContentPage
    {
        public ReportBugPage()
        {
            InitializeComponent();
        }
private async void OnSubmitBugReportClicked(object sender, EventArgs e)
{
    // Get the bug description from the Editor
    string bugDescription = BugDescriptionEditor.Text;

    // Check if the text box is empty
    if (string.IsNullOrEmpty(bugDescription))
    {
        await DisplayAlert("Error", "Please enter a description of the issue.", "OK");
        return;
    }

    // Capture Device Information (optional)
    string deviceInfo = $"Device: {DeviceInfo.Name}, OS: {DeviceInfo.Platform}, Version: {DeviceInfo.VersionString}";

    // Construct the email body
    string emailBody = $"{bugDescription}\n\nDevice Information:\n{deviceInfo}";

    // Open default email client with pre-filled data
    try
    {
        var message = new EmailMessage
        {
            Subject = "Bug Report from LogBotics App",
            Body = emailBody,
            To = new List<string> { "jrigdon@purdue.edu" } //will need all pof our emails eventually
        };

        await Email.ComposeAsync(message);
        await DisplayAlert("Success", "Bug report has been sent successfully!", "OK");
    }
    catch (FeatureNotSupportedException)
    {
        await DisplayAlert("Error", "Email is not supported on this device.", "OK");
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to send bug report: {ex.Message}", "OK");
    }
}

    }
}
