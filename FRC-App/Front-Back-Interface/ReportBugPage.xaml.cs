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
            string bugDescription = BugDescriptionEditor.Text;

            if (string.IsNullOrEmpty(bugDescription))
            {
                await DisplayAlert("Error", "Please enter a description of the issue.", "OK");
                return;
            }

            // Capture Device Information (example)
            string deviceInfo = $"Device: {DeviceInfo.Name}, OS: {DeviceInfo.Platform}, Version: {DeviceInfo.VersionString}";

            // Construct Email
            string subject = "Bug Report from LogBotics App";
            string body = $"{bugDescription}\n\nDevice Info:\n{deviceInfo}";

            try
            {
                // Send Email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("user@example.com"), // Change this to an appropriate email address
                    Subject = subject,
                    Body = body
                };
                mailMessage.To.Add("team24@purdue.edu"); // Developer email

                // Use a library or SMTP client for sending (this example uses SMTP)
                SmtpClient smtpClient = new SmtpClient("smtp.yourmailserver.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("username", "password"),
                    EnableSsl = true
                };
                
                smtpClient.Send(mailMessage);
                await DisplayAlert("Success", "Bug report has been sent successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send bug report: {ex.Message}", "OK");
            }
        }
    }
}
