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
            
        }
    }
}