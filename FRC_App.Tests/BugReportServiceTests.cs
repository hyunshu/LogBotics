using FRC_App.Services;
using Moq;
using Xunit;

namespace FRC_App.Tests
{
    public class BugReportServiceTests
    {
        [Fact]
        public async Task SendBugReportAsync_EmptyBugDescription_ReturnsError()
        {
            // Arrange
            var mockDeviceInfoProvider = new Mock<IDeviceInfoProvider>();
            var service = new BugReportService(mockDeviceInfoProvider.Object);

            // Act
            var result = await service.SendBugReportAsync("");

            // Assert
            Assert.Equal("Error: Please enter a description of the bug.", result);
        }

        [Fact]
        public async Task SendBugReportAsync_ValidBugDescription_ReturnsSuccess()
        {
            // Arrange
            var mockDeviceInfoProvider = new Mock<IDeviceInfoProvider>();
            mockDeviceInfoProvider.Setup(x => x.Name).Returns("Test Device");
            mockDeviceInfoProvider.Setup(x => x.Platform).Returns("Test OS");
            mockDeviceInfoProvider.Setup(x => x.VersionString).Returns("1.0");

            var service = new BugReportService(mockDeviceInfoProvider.Object);

            // Act
            var result = await service.SendBugReportAsync("Test bug description");

            // Assert
            Assert.StartsWith("Success", result);
        }
        /*
        [Fact]
        public async Task SendBugReportAsync_EmailSendingFails_ReturnsError()
        {
            // Arrange
            var mockDeviceInfoProvider = new Mock<IDeviceInfoProvider>();
            mockDeviceInfoProvider.Setup(x => x.Name).Returns("Test Device");
            mockDeviceInfoProvider.Setup(x => x.Platform).Returns("Test OS");
            mockDeviceInfoProvider.Setup(x => x.VersionString).Returns("1.0");

            var service = new BugReportService(mockDeviceInfoProvider.Object);

            // Simulate failure by providing invalid SMTP credentials.
            var result = await service.SendBugReportAsync("Simulated failure");

            // Assert
            Assert.StartsWith("Error", result);
        }
        */
    }
}
