using Xunit;
using Moq;
using System.Collections.Generic;
using FRC_App.Services; // Assuming parsing logic is in a service
using FRC_App.Models;  // Assuming errors are stored in a model

public class DebuggingTests
{
    // Test to verify the parsing algorithm finds errors in a .dsevents file
    [Fact]
    public void ParseDseventsFile_FindsErrors()
    {
        // Arrange
        var sampleDseventsContent = @"
        [Timestamp] ERROR: Communication lost
        [Timestamp] ERROR: CAN Bus error
        ";
        var parser = new DseventsParser(); // Replace with your actual parsing class

        // Act
        var errors = parser.Parse(sampleDseventsContent);

        // Assert
        Assert.NotNull(errors);
        Assert.Equal(2, errors.Count);
        Assert.Contains("Communication lost", errors);
        Assert.Contains("CAN Bus error", errors);
    }

    // Test to verify no errors found in a clean .dsevents file
    [Fact]
    public void ParseDseventsFile_NoErrorsFound()
    {
        // Arrange
        var sampleDseventsContent = @"
        [Timestamp] INFO: All systems operational
        ";
        var parser = new DseventsParser();

        // Act
        var errors = parser.Parse(sampleDseventsContent);

        // Assert
        Assert.NotNull(errors);
        Assert.Empty(errors);
    }

    // Test to verify that errors are displayed correctly in the Debugging window
    [Fact]
    public void DisplayErrors_ShowsCorrectMessages()
    {
        // Arrange
        var errors = new List<string> { "Communication lost", "CAN Bus error" };
        var viewModel = new DebuggingViewModel(); // Replace with your actual view model class

        // Act
        viewModel.SetErrors(errors);

        // Assert
        Assert.Equal(2, viewModel.ErrorMessages.Count);
        Assert.Contains("Communication lost", viewModel.ErrorMessages);
        Assert.Contains("CAN Bus error", viewModel.ErrorMessages);
    }

    // Test to verify feedback when a file upload is successful
    [Fact]
    public void UploadFile_SuccessfulUpload()
    {
        // Arrange
        var debuggingPage = new DebuggingPage(); // Replace with your actual page or logic class
        var filePath = "path/to/sample.dsevents";

        // Act
        var result = debuggingPage.UploadFile(filePath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("File uploaded successfully.", result.Message);
    }

    // Test to verify feedback when a file upload fails
    [Fact]
    public void UploadFile_UnsuccessfulUpload()
    {
        // Arrange
        var debuggingPage = new DebuggingPage();
        var invalidFilePath = "path/to/invalid.dsevents";

        // Act
        var result = debuggingPage.UploadFile(invalidFilePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to upload file. Please try again.", result.Message);
    }
}
