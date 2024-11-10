using Microsoft.Maui.Controls;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;

namespace FRC_App
{
    public partial class ReportBugPage : ContentPage
    {
        public ReportBugPage()
        {
            InitializeComponent();
        }
   /* private async void OnSubmitBugReportClicked(object sender, EventArgs e)
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
    */
    private async void SendBugReportAutomatically(object sender, EventArgs e)
    {
        string bugDescription = BugDescriptionEntry.Text;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("LogBotics App", "logboticsteam@gmail.com"));
        message.To.Add(new MailboxAddress("Team 24", "jrigdon@purdue.edu"));
        message.Subject = "Bug Report from LogBotics App";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"{bugDescription}\n\nDevice Information:\n" +
                    $"Device: {DeviceInfo.Name}, OS: {DeviceInfo.Platform}, Version: {DeviceInfo.VersionString}"
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("logboticsteam@gmail.com", "wfso mcdw gmie wngx"); // Use app password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
                await DisplayAlert("Success", "Bug report has been sent successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to send bug report: {ex.Message}", "OK");
        }
    }

    }
}
