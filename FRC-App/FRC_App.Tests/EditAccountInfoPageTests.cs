/*
using System;
using Moq;
using Xunit;
using FRC_App;
using FRC_App.Services;
using FRC_App.Models;

namespace FRC_App.Tests
{
    public class EditAccountInfoPageTests
    {
        private readonly Mock<IUserDatabase> _userDatabaseMock;
        private readonly User _currentUser;
        private readonly EditAccountInfoPage _editAccountInfoPage;

        public EditAccountInfoPageTests()
        {
            // Mock the UserDatabase service
            _userDatabaseMock = new Mock<IUserDatabase>();

            // Create a test user
            _currentUser = new User
            {
                Username = "testUser",
                TeamName = "Test Team",
                TeamNumber = "25"
            };

            // Instantiate the EditAccountInfoPage with the test user
            _editAccountInfoPage = new EditAccountInfoPage(_currentUser);
        }

        [Fact]
        public async void OnSaveTeamNameClicked_ValidTeamName_ShouldUpdateTeamName()
        {
            // Arrange
            _editAccountInfoPage.teamNameEntry.Text = "New Team Name";
            _editAccountInfoPage.confirmTeamNameEntry.Text = "New Team Name";

            _userDatabaseMock.Setup(x => x.UpdateTeamName(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            _editAccountInfoPage.OnSaveTeamNameClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateTeamName(_currentUser, "New Team Name"), Times.Once);
        }

        [Fact]
        public async void OnSaveTeamNameClicked_TeamNamesDoNotMatch_ShouldNotUpdateTeamName()
        {
            // Arrange
            _editAccountInfoPage.teamNameEntry.Text = "Team Name";
            _editAccountInfoPage.confirmTeamNameEntry.Text = "Different Team Name";

            // Act
            _editAccountInfoPage.OnSaveTeamNameClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateTeamName(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void OnSaveTeamNumberClicked_ValidTeamNumber_ShouldUpdateTeamNumber()
        {
            // Arrange
            _editAccountInfoPage.teamNumberEntry.Text = "42";
            _editAccountInfoPage.confirmTeamNumberEntry.Text = "42";

            _userDatabaseMock.Setup(x => x.UpdateTeamNumber(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            _editAccountInfoPage.OnSaveTeamNumberClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateTeamNumber(_currentUser, "42"), Times.Once);
        }

        [Fact]
        public async void OnSaveTeamNumberClicked_TeamNumbersDoNotMatch_ShouldNotUpdateTeamNumber()
        {
            // Arrange
            _editAccountInfoPage.teamNumberEntry.Text = "42";
            _editAccountInfoPage.confirmTeamNumberEntry.Text = "43";

            // Act
            _editAccountInfoPage.OnSaveTeamNumberClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateTeamNumber(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void OnSaveUsernameClicked_ValidUsername_ShouldUpdateUsername()
        {
            // Arrange
            _editAccountInfoPage.usernameEntry.Text = "newUsername";
            _editAccountInfoPage.confirmUsernameEntry.Text = "newUsername";

            _userDatabaseMock.Setup(x => x.UpdateUsername(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            _editAccountInfoPage.OnSaveUsernameClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateUsername(_currentUser, "newUsername"), Times.Once);
        }

        [Fact]
        public async void OnSaveUsernameClicked_UsernamesDoNotMatch_ShouldNotUpdateUsername()
        {
            // Arrange
            _editAccountInfoPage.usernameEntry.Text = "newUsername";
            _editAccountInfoPage.confirmUsernameEntry.Text = "differentUsername";

            // Act
            _editAccountInfoPage.OnSaveUsernameClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdateUsername(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void OnSavePasswordClicked_ValidPassword_ShouldUpdatePassword()
        {
            // Arrange
            _editAccountInfoPage.passwordEntry.Text = "newPassword";
            _editAccountInfoPage.confirmPasswordEntry.Text = "newPassword";

            _userDatabaseMock.Setup(x => x.UpdatePassword(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            _editAccountInfoPage.OnSavePasswordClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdatePassword(_currentUser, "newPassword"), Times.Once);
        }

        [Fact]
        public async void OnSavePasswordClicked_PasswordsDoNotMatch_ShouldNotUpdatePassword()
        {
            // Arrange
            _editAccountInfoPage.passwordEntry.Text = "newPassword";
            _editAccountInfoPage.confirmPasswordEntry.Text = "differentPassword";

            // Act
            _editAccountInfoPage.OnSavePasswordClicked(null, EventArgs.Empty);

            // Assert
            _userDatabaseMock.Verify(x => x.UpdatePassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }
}
*/