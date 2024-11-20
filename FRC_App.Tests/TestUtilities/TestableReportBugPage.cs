using Microsoft.Maui.Controls;

namespace FRC_App.Tests.TestUtilities
{
    public class TestableReportBugPage : ReportBugPage
    {
        public string? LastDisplayedAlertMessage { get; private set; } = string.Empty;
        public string? LastSentEmailBody { get; private set; } = string.Empty;
        public bool WasEmailSent { get; private set; }
        public bool SimulateEmailFailure { get; set; }

        public Editor BugDescriptionEditor { get; } = new Editor();

        public async Task SimulateSendBugReport(string description)
        {
            BugDescriptionEditor.Text = description;

            if (string.IsNullOrWhiteSpace(description))
            {
                LastDisplayedAlertMessage = "Please enter a description of the bug.";
                return;
            }

            try
            {
                await SimulateSendEmail("Bug Report from LogBotics App", description, "jrigdon@purdue.edu");
                LastDisplayedAlertMessage = "Bug report has been sent successfully!";
            }
            catch
            {
                LastDisplayedAlertMessage = "Failed to send bug report.";
            }
        }

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
