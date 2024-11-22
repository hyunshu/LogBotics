using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using FRC_App.Models; // Adjust to the correct namespace

namespace FRC_App.Tests
{
    public class DataImportTests
    {
        private DataImport _dataImport;

        public DataImportTests()
        {
            _dataImport = new DataImport();
        }

        [Fact]
        public void Constructor_Parameterized_SetsProperties()
        {
            // Arrange
            string sessionName = "TestSession";
            var dataTypes = new List<string> { "Motor", "Sensor" };
            var dataUnits = new List<List<string>> { new List<string> { "Time (s)", "Velocity (m/s)" } };

            // Act
            var dataImport = new DataImport(sessionName, dataTypes, dataUnits);

            // Assert
            Assert.Equal(sessionName, dataImport.sessionName);
            Assert.Equal(dataTypes, dataImport.dataTypes);
            Assert.Equal(dataUnits, dataImport.dataUnits);
        }

        [Fact]
        public void Constructor_Default_PopulatesTestData()
        {
            // Act
            var dataImport = new DataImport();

            // Assert
            Assert.NotNull(dataImport.dataTypes);
            Assert.NotEmpty(dataImport.dataTypes);
            Assert.NotNull(dataImport.dataUnits);
            Assert.NotEmpty(dataImport.dataUnits);
        }

        [Fact]
        public void GenerateTestData_ProvidesValidData()
        {
            // Act
            var rawData = _dataImport.GenerateTestData();

            // Assert
            Assert.NotNull(rawData);
            Assert.NotEmpty(rawData); // Ensure data is generated
            Assert.Equal(4, rawData.Count); // Check number of data types
        }

        [Fact]
public void FromRobot_ParsesRobotDataFile()
{
    string filePath = "test_robot_data.txt";

    try
    {
        // Arrange
        File.WriteAllLines(filePath, new[]
        {
            "Motor:Time (s):0.1",
            "Motor:Velocity (m/s):1.2",
            "Sensor:Angle (rad):0.5"
        });

        // Act
        var rawData = _dataImport.FromRobot(filePath);

        // Assert
        Assert.NotNull(rawData);
        Assert.NotEmpty(rawData); // Ensure data is parsed
        Assert.Equal(2, rawData.Count); // Two data types: Motor, Sensor
        Assert.Equal(2, rawData[0].Count); // Motor has 2 columns
    }
    finally
    {
        // Cleanup
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}


        [Fact]
        public void FromCSV_ParsesMultipleCSVFiles()
        {
            // Arrange
            string file1 = "TestSession_Motor.csv";
            string file2 = "TestSession_Sensor.csv";

            File.WriteAllText(file1, "Time (s),Velocity (m/s)\n0.1,1.2");
            File.WriteAllText(file2, "Time (s),Angle (rad)\n0.1,0.5");

            var filePaths = new List<string> { file1, file2 };

            // Act
            var rawData = _dataImport.FromCSV(filePaths);

            // Assert
            Assert.NotNull(rawData);
            Assert.NotEmpty(rawData); // Ensure data is parsed
            Assert.Equal(2, rawData.Count); // Two data types: Motor, Sensor
            Assert.Equal(1, rawData[0][0].Count); // Motor data has 1 row
            File.Delete(file1); // Clean up
            File.Delete(file2);
        }

        [Fact]
        public void StoreRawData_SavesDataCorrectly()
        {
            // Arrange
            var rawData = _dataImport.GenerateTestData();
            var user = new User();

            // Act
            _dataImport.StoreRawData(rawData, user);

            // Assert
            Assert.NotNull(user.sessions);
            Assert.Contains(_dataImport.sessionName, user.sessions);
        }

        [Fact]
        public void RetrieveRawData_RetrievesDataFromUserObject()
        {
            // Arrange
            var rawData = _dataImport.GenerateTestData();
            var user = new User();
            _dataImport.StoreRawData(rawData, user);

            // Act
            var retrievedData = _dataImport.RetrieveRawData(user, _dataImport.sessionName);

            // Assert
            Assert.NotNull(retrievedData);
            Assert.Equal(rawData.Count, retrievedData.Count); // Check number of data types
        }

        [Fact]
        public void RetrieveRawData_NoMatchingSession_ThrowsAnyException()
        {
            // Arrange
            var user = new User();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
            {
                _dataImport.RetrieveRawData(user, "NonExistentSession");
            });
        }


        [Fact]
        public void FromCSV_InvalidFileName_ThrowsException()
        {
            // Arrange
            var invalidFilePaths = new List<string> { "invalid_filename.csv" };

            // Act & Assert
            Assert.Throws<Exception>(() => _dataImport.FromCSV(invalidFilePaths));
        }
    }
}
