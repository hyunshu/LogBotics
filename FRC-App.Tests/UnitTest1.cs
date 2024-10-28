using NUnit.Framework; // Required for NUnit attributes and assertions
using FRC_App; // Namespace of your main project
using FRC_App.Models; // Assuming the User model is here

namespace FRC_App.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private HomePage homePage;

        [SetUp]
        public void Setup()
        {
            // Create a test user to pass to HomePage
            User testUser = new User
            {
                Username = "TestUser",
                TeamNumber = "1234",
                TeamName = "Test Team",
                rawData = "some_raw_data" // Initial raw data for testing purposes
            };

            // Initialize HomePage with a test user
            homePage = new HomePage(testUser);
        }

        [Test]
        public void LoadUserData_WithNoData_ShouldHideCharts()
        {
            // Arrange: Set rawData to null to simulate no data
            homePage.currentUser.rawData = null;

            // Act: Call the method under test
            homePage.loadUserData();

            // Assert: Ensure the charts are hidden when there's no data
            Assert.That(homePage.chartView1.IsVisible, Is.False, "ChartView1 should be hidden when there is no data.");
            Assert.That(homePage.chartView2.IsVisible, Is.False, "ChartView2 should be hidden when there is no data.");
            Assert.That(homePage.chartView3.IsVisible, Is.False, "ChartView3 should be hidden when there is no data.");
        }

        [Test]
        public void LoadUserData_WithData_ShouldShowCharts()
        {
            // Arrange: Set rawData to a valid value
            homePage.currentUser.rawData = "some_valid_data";

            // Act: Call the method under test
            homePage.loadUserData();

            // Assert: Ensure the charts are visible when data is present
            Assert.That(homePage.chartView1.IsVisible, Is.True, "ChartView1 should be visible when data is present.");
            Assert.That(homePage.chartView2.IsVisible, Is.True, "ChartView2 should be visible when data is present.");
            Assert.That(homePage.chartView3.IsVisible, Is.True, "ChartView3 should be visible when data is present.");
        }
    }
}

