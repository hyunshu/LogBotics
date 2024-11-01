using System;
using FRC_App;
using FRC_App.Models;
using FRC_App.Services;
using Xunit;

namespace FRC_App
{
    public partial class UnitTestPage : ContentPage
    {
        public UnitTestPage()
        {
            InitializeComponent();
        }

        private async void OnEmptyTestClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Test", "Empty test executed.", "OK");
        }

        private async void OnUpdateTeamName(object sender, EventArgs e)
        {
            try
            {
                // Arrange
                await UserDatabase.AddUser("TestTeamName", "1", "TestUsername", "TestPassword", "TestSecurityQuestion", "TestSecurityAnswer", false);

                // Act
                User user = await UserDatabase.db.Table<User>().Where(u => u.TeamName == "TestTeamName").FirstOrDefaultAsync();
                int id = user.Id;
                await UserDatabase.UpdateTeamName(user, "UpdatedTeamName");

                User actualUser = await UserDatabase.db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
                
                // Assert
                Assert.Equal("UpdatedTeamName", actualUser.TeamName);

                UserDatabase.db.DeleteAllAsync<User>();

                // If it passes, show a success alert
                await DisplayAlert("Test Passed", "The team name was updated successfully.", "OK");
            }
            catch (Exception ex)
            {
                // If it fails, show a failure alert with the error message
                UserDatabase.db.DeleteAllAsync<User>();
                await DisplayAlert("Test Failed", $"The team name update test failed. Error: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateTeamNumber(object sender, EventArgs e)
        {
            try
            {
                // Arrange
                await UserDatabase.AddUser("TestTeamName", "1", "TestUsername", "TestPassword", "TestSecurityQuestion", "TestSecurityAnswer", false);

                // Act
                User user = await UserDatabase.db.Table<User>().Where(u => u.TeamName == "TestTeamName").FirstOrDefaultAsync();
                int id = user.Id;
                await UserDatabase.UpdateTeamNumber(user, "2");

                User actualUser = await UserDatabase.db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
                
                // Assert
                Assert.Equal("2", actualUser.TeamNumber);

                UserDatabase.db.DeleteAllAsync<User>();

                // If it passes, show a success alert
                await DisplayAlert("Test Passed", "The team number was updated successfully.", "OK");
            }
            catch (Exception ex)
            {
                // If it fails, show a failure alert with the error message
                UserDatabase.db.DeleteAllAsync<User>();
                await DisplayAlert("Test Failed", $"The team number update test failed. Error: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateUsername(object sender, EventArgs e)
        {
            try
            {
                // Arrange
                await UserDatabase.AddUser("TestTeamName", "1", "TestUsername", "TestPassword", "TestSecurityQuestion", "TestSecurityAnswer", false);

                // Act
                User user = await UserDatabase.db.Table<User>().Where(u => u.TeamName == "TestTeamName").FirstOrDefaultAsync();
                int id = user.Id;
                await UserDatabase.UpdateUsername(user, "UpdatedUsername");

                User actualUser = await UserDatabase.db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
                
                // Assert
                Assert.Equal("UpdatedUsername", actualUser.Username);

                UserDatabase.db.DeleteAllAsync<User>();

                // If it passes, show a success alert
                await DisplayAlert("Test Passed", "The username was updated successfully.", "OK");
            }
            catch (Exception ex)
            {
                // If it fails, show a failure alert with the error message
                UserDatabase.db.DeleteAllAsync<User>();
                await DisplayAlert("Test Failed", $"The username update test failed. Error: {ex.Message}", "OK");
            }
        }

        private async void OnUpdatePassword(object sender, EventArgs e)
        {
            try
            {
                // Arrange
                await UserDatabase.AddUser("TestTeamName", "1", "TestUsername", "TestPassword", "TestSecurityQuestion", "TestSecurityAnswer", false);

                // Act
                User user = await UserDatabase.db.Table<User>().Where(u => u.TeamName == "TestTeamName").FirstOrDefaultAsync();
                int id = user.Id;
                await UserDatabase.UpdatePassword(user, "UpdatedPassword");

                User actualUser = await UserDatabase.db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
                
                // Assert
                Assert.Equal("UpdatedPassword", actualUser.Password);

                UserDatabase.db.DeleteAllAsync<User>();

                // If it passes, show a success alert
                await DisplayAlert("Test Passed", "The password was updated successfully.", "OK");
            }
            catch (Exception ex)
            {
                // If it fails, show a failure alert with the error message
                UserDatabase.db.DeleteAllAsync<User>();
                await DisplayAlert("Test Failed", $"The password update test failed. Error: {ex.Message}", "OK");
            }
        }
    }
}
