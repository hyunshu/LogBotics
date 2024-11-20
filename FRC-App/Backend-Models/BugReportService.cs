using MailKit.Net.Smtp;
using MimeKit;

namespace FRC_App.Services
{
    public class BugReportService
    {
        private const string SenderEmail = "logboticsteam@gmail.com";
        private const string AppPassword = "wfso mcdw gmie wngx";
        private const string RecipientEmail = "jrigdon@purdue.edu";

        public async Task<string> SendBugReportAsync(string bugDescription)
        {
            if (string.IsNullOrWhiteSpace(bugDescription))
            {
                return "Error: Please enter a description of the bug.";
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LogBotics App", SenderEmail));
            message.To.Add(new MailboxAddress("Team 24", RecipientEmail));
            message.Subject = "Bug Report from LogBotics App";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"{bugDescription}\n\nDevice Information:\n" +
                           $"Device: {DeviceInfo.Name}, OS: {DeviceInfo.Platform}, Version: {DeviceInfo.VersionString}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync(SenderEmail, AppPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return "Success: Bug report has been sent successfully!";
            }
            catch (Exception ex)
            {
                return $"Error: Failed to send bug report: {ex.Message}";
            }
        }
    }
}
