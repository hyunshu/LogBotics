using Xunit;
using System.Threading.Tasks;

public class ReportBugPageTests
{
    // Mock DeviceInfo (can vary based on the platform)
    private const string MockDeviceName = "Test Device";
    private const string MockDevicePlatform = "Test OS";
    private const string MockDeviceVersion = "1.0";

    [Fact]
    public async Task SendBugReportAutomatically_EmptyBugDescription_ShowsErrorMessage()
    {
        // Arrange
        var page = new TestableReportBugPage();
        var editor = page.FindByName<Editor>("BugDescriptionEditor");
        editor.Text = ""; // Simulate empty input

        // Act
        await page.SendBugReportAutomatically(null, null);

        // Assert
        Assert.Equal("Please enter a description of the bug.", page.DisplayedAlertMessage);
    }

    [Fact]
    public async Task SendBugReportAutomatically_ValidInput_EmailSentSuccessfully()
    {
        // Arrange
        var page = new TestableReportBugPage();
        var editor = page.FindByName<Editor>("BugDescriptionEditor");
        editor.Text = "This is a test bug report.";

        // Act
        await page.SendBugReportAutomatically(null, null);

        // Assert
        Assert.Equal("Bug report has been sent successfully!", page.DisplayedAlertMessage);
        Assert.Contains("This is a test bug report.", page.LastSentEmailBody);
        Assert.Contains($"Device: {MockDeviceName}, OS: {MockDevicePlatform}, Version: {MockDeviceVersion}", page.LastSentEmailBody);
    }

    [Fact]
    public async Task SendBugReportAutomatically_EmailSendingFails_ShowsErrorMessage()
    {
        // Arrange
        var page = new TestableReportBugPage();
        page.SimulateEmailFailure = true; // Simulate email failure
        var editor = page.FindByName<Editor>("BugDescriptionEditor");
        editor.Text = "This is a test bug report.";

        // Act
        await page.SendBugReportAutomatically(null, null);

        // Assert
        Assert.StartsWith("Failed to send bug report:", page.DisplayedAlertMessage);
        Assert.False(page.WasEmailSent); // Ensure email was not sent
    }
}
