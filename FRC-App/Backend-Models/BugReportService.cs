using MailKit.Net.Smtp;
using MimeKit;
namespace FRC_App.Services
{
    public class BugReportService
    {
        private readonly IDeviceInfoProvider _deviceInfoProvider;

        public BugReportService(IDeviceInfoProvider deviceInfoProvider)
        {
            _deviceInfoProvider = deviceInfoProvider;
        }

        public async Task<string> SendBugReportAsync(string bugDescription)
        {
            if (string.IsNullOrWhiteSpace(bugDescription))
            {
                return "Error: Please enter a description of the bug.";
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LogBotics App", "logboticsteam@gmail.com"));
            message.To.Add(new MailboxAddress("Team 24", "jrigdon@purdue.edu"));
            message.Subject = "Bug Report from LogBotics App";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"{bugDescription}\n\nDevice Information:\n" +
                           $"Device: {_deviceInfoProvider.Name}, OS: {_deviceInfoProvider.Platform}, Version: {_deviceInfoProvider.VersionString}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("logboticsteam@gmail.com", "wfso mcdw gmie wngx");
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
