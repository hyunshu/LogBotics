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
   
    private async void SendBugReportAutomatically(object sender, EventArgs e)
    {
        string bugDescription = BugDescriptionEditor.Text;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("LogBotics App", "logboticsteam@gmail.com"));
        message.To.Add(new MailboxAddress("Team 24", "jrigdon@purdue.edu"));
        message.Subject = "Bug Report from LogBotics App";

        if (string.IsNullOrWhiteSpace(bugDescription))
        {
        await DisplayAlert("Error", "Please enter a description of the bug.", "OK");
        return;
        }

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
