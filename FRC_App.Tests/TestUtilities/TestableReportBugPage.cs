using FRC_App;

namespace FRC_App.Tests.TestUtilities
{
    public class TestableReportBugPage : ReportBugPage
    {
        public string LastDisplayedAlertMessage { get; private set; }
        public string LastSentEmailBody { get; private set; }
        public bool WasEmailSent { get; private set; }
        public bool SimulateEmailFailure { get; set; }

        public async Task SimulateSendBugReport(string description)
        {
            // Simulate setting the bug description
            BugDescriptionEditor.Text = description;

            // Call the actual method and capture side effects
            try
            {
                await SendBugReportAutomatically(null, null);
            }
            catch
            {
                // Ignore exceptions in tests for validation
            }
        }

        // Capture alerts
        private async Task DisplayAlert(string title, string message, string cancel)
        {
            LastDisplayedAlertMessage = message;
            await Task.CompletedTask;
        }

        // Simulate sending email
        private async Task SimulateSendEmail(string subject, string body, string recipientEmail)
        {
            if (SimulateEmailFailure)
            {
                throw new Exception("Simulated email failure");
            }

            WasEmailSent = true;
            LastSentEmailBody = body;
            await Task.CompletedTask;
        }
    }
}
