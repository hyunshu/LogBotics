/*
using NUnit.Framework;
using FRC_App.Models;
using FRC_App.Services;
using System.Threading.Tasks;

namespace FRC_App.Tests
{
    [TestFixture]
    public class EditAccountInfoTests
    {
        private User testUser;

        [SetUp]
        public async Task Setup()
        {
            testUser = new User
            {
                TeamName = "TestTeam",
                TeamNumber = "1234",
                Username = "TestUser",
                Password = "TestPass123"
            };

            await UserDatabase.AddUser(
                teamName: testUser.TeamName,
                teamNumber: testUser.TeamNumber,
                name: testUser.Username,
                password: testUser.Password,
                securityQuestion: "What is your pet's name?",
                securityAnswer: "Fluffy",
                isAdmin: false
            );
        }

        [Test]
        public async Task OnSaveTeamNameClicked_ShouldUpdateTeamNameInDatabase()
        {
            string newTeamName = "UpdatedTeamName";
            await UserDatabase.UpdateTeamName(testUser, newTeamName);

            var updatedUser = await UserDatabase.GetUser(testUser.Username);
            Assert.AreEqual(newTeamName, updatedUser.TeamName);
        }

        [TearDown]
        public async Task TearDown()
        {
            var user = await UserDatabase.GetUser(testUser.Username);
            if (user != null)
            {
                await UserDatabase.db.DeleteAsync(user);
            }
        }
    }
}
*/