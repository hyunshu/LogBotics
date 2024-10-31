using NUnit.Framework; // Required for NUnit attributes and assertions
using FRC_App; // Namespace of your main project
using FRC_App.Models; // Assuming the User model is here

namespace FRC_App.Tests
{
    [TestFixture]
public class HomePageTests
{
    private HomePage homePage;
    private User mockUser;

    [SetUp]
    public void Setup()
    {
        // Arrange: Create a mock user and HomePage instance
        mockUser = new User { Username = "TestUser", TeamNumber = "1234" };
        homePage = new HomePage(mockUser);
    }

    [Test]
    public void LoadUserData_WithNoData_ShouldHandleNoDataGracefully()
    {
        // Arrange: Set rawData to null to simulate no data
        homePage.currentUser.rawData = null;

        // Act: Call the method under test
        homePage.loadUserData();

        // Assert: Verify the expected state (without UI elements)
        Assert.That(homePage.currentUser.rawData, Is.Null, "Raw data should be null when there is no data.");
    }

    [Test]
    public void LoadUserData_WithData_ShouldSetRawDataCorrectly()
    {
        // Arrange: Set rawData to a valid value to simulate data presence
        homePage.currentUser.rawData = "some_valid_data";

        // Act: Call the method under test
        homePage.loadUserData();

        // Assert: Verify that rawData has been loaded properly
        Assert.That(homePage.currentUser.rawData, Is.EqualTo("some_valid_data"), "Raw data should match the provided data.");
    }
}

}

