using Xunit;
using FRC_App.Utilities;

public class DseventsParserTests
{
    [Fact]
    public void Parse_FindsErrorsAndWarnings()
    {
        // Arrange
        string fileContent = @"
            [2024-01-01 12:00:00] ERROR: Lost communication with device
            [2024-01-01 12:01:00] WARNING: High latency detected
        ";

        // Act
        var result = DseventsParser.Parse(fileContent);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("Lost communication with device", result);
        Assert.Contains("Warning:", result);
        Assert.Contains("High latency detected", result);
    }

    [Fact]
    public void Parse_NoErrorsOrWarnings()
    {
        // Arrange
        string fileContent = @"
            [2024-01-01 12:00:00] INFO: All systems operational
        ";

        // Act
        var result = DseventsParser.Parse(fileContent);

        // Assert
        Assert.Equal("No errors or warnings found.", result);
    }

    [Fact]
    public void Parse_EmptyFileContent()
    {
        // Arrange
        string emptyContent = "";

        // Act
        var result = DseventsParser.Parse(emptyContent);

        // Assert
        Assert.Equal("No errors or warnings found.", result);
    }

    [Fact]
    public void Parse_FindsMultipleErrors()
    {
        // Arrange
        string fileContent = @"
            [2024-01-01 12:00:00] ERROR: Device failed to respond
            [2024-01-01 12:01:00] ERROR: Lost communication with device
        ";

        // Act
        var result = DseventsParser.Parse(fileContent);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("Device failed to respond", result);
        Assert.Contains("Lost communication with device", result);
    }
}
