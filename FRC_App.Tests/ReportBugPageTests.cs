using Xunit;
using FRC_App.Tests.TestUtilities;

public class ReportBugPageTests
{
    [Fact]
    public async Task SendBugReportAutomatically_EmptyBugDescription_ShowsErrorMessage()
    {
        var page = new TestableReportBugPage();
        await page.SimulateSendBugReport(""); // Empty description

        Assert.Equal("Please enter a description of the bug.", page.LastDisplayedAlertMessage);
    }

    [Fact]
    public async Task SendBugReportAutomatically_ValidInput_EmailSentSuccessfully()
    {
        var page = new TestableReportBugPage();
        await page.SimulateSendBugReport("This is a test bug report.");

        Assert.Equal("Bug report has been sent successfully!", page.LastDisplayedAlertMessage);
        Assert.Contains("This is a test bug report.", page.LastSentEmailBody);
    }

    [Fact]
    public async Task SendBugReportAutomatically_EmailFailure_ShowsErrorMessage()
    {
        var page = new TestableReportBugPage { SimulateEmailFailure = true };
        await page.SimulateSendBugReport("This is a test bug report.");

        Assert.Equal("Failed to send bug report.", page.LastDisplayedAlertMessage);
        Assert.False(page.WasEmailSent);
    }
}
