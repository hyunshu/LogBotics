using System;
using System.Collections.Generic;
using Xunit;
using FRC_App.Models;

namespace FRC_App.Tests
{
    public class DataContainerTests
    {
        [Fact]
        public void Constructor_WithValidUser_ParsesSessions()
        {
            // Arrange
            var dataContainer  = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });
            dataContainer.addSession(new Session { Name = "Session2" });
            dataContainer.addSession(new Session { Name = "Session3" });

            // Act
            var sessionNames = dataContainer.getSessionNames();

            // Assert
            var expected = new List<string> { "Session1", "Session2", "Session3" };

            Assert.NotNull(sessionNames);
            Assert.Equal(expected.Count, sessionNames.Count);
            Assert.Equal(expected, sessionNames);
        }

        [Fact]
        public void Constructor_WithEmptySessions_InitializesEmptyList()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});

            // Act
            var sessionNames = dataContainer.getSessionNames();

            // Assert
            Assert.NotNull(sessionNames);
            Assert.Empty(sessionNames);
        }

        [Fact]
        public void GetSessionNames_ReturnsCorrectNames()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });
            dataContainer.addSession(new Session { Name = "Session2" });

            // Act
            var sessionNames = dataContainer.getSessionNames();

            // Assert
            var expected = new List<string> { "Session1", "Session2" };
            Assert.NotNull(sessionNames);
            Assert.Equal(expected.Count, sessionNames.Count);
            Assert.Equal(expected, sessionNames);
        }

        [Fact]
        public void GetSession_WithExistingName_ReturnsSession()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });
            dataContainer.addSession(new Session { Name = "Session2" });

            // Act
            var session = dataContainer.getSession("Session1");

            // Assert
            Assert.NotNull(session);
            Assert.Equal("Session1", session.Name);
        }

        [Fact]
        public void GetSession_WithNonExistingName_ReturnsNull()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });
            dataContainer.addSession(new Session { Name = "Session2" });

            // Act
            var session = dataContainer.getSession("NonExistingSession");

            // Assert
            Assert.Null(session);
        }

        [Fact]
        public void AddSession_AddsNewSession()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });

            var newSession = new Session { Name = "Session2" };

            // Act
            dataContainer.addSession(newSession);
            var sessionNames = dataContainer.getSessionNames();

            // Assert
            var expected = new List<string> { "Session1", "Session2" };
            Assert.Equal(expected.Count, sessionNames.Count);
            Assert.Equal(expected, sessionNames);
        }

        [Fact]
        public void RemoveSession_RemovesExistingSession()
        {
            // Arrange
            var dataContainer = new DataContainer(new User{});
            dataContainer.addSession(new Session { Name = "Session1" });
            dataContainer.addSession(new Session { Name = "Session2" });
            
            var session = dataContainer.getSession("Session1");

            // Act
            dataContainer.removeSession(session);
            var sessionNames = dataContainer.getSessionNames();

            // Assert
            var expected = new List<string> { "Session2" };
            Assert.Equal(expected.Count, sessionNames.Count);
            Assert.Equal(expected, sessionNames);
        }
    }
}
