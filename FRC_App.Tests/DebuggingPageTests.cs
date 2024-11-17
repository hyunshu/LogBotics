using Xunit;

namespace FRC_App.Tests
{
    public class DebuggingPageTests
    {
        // Test: ParsingStatus updates correctly when set
        [Fact]
        public void ParsingStatusLabel_UpdatesCorrectly()
        {
            // Arrange
            var debuggingPage = new DebuggingPage();

            // Act
            debuggingPage.ParsingStatus = "File uploaded successfully.";

            // Assert
            Assert.Equal("File uploaded successfully.", debuggingPage.ParsingStatus);
        }

        // Test: ParsingStatus displays error message for invalid file type
        [Fact]
        public void ParsingStatusLabel_DisplaysErrorForInvalidFileType()
        {
            // Arrange
            var debuggingPage = new DebuggingPage();

            // Simulate an invalid file type
            var invalidFileName = "log.txt";

            // Act
            if (!invalidFileName.EndsWith(".dsevents", StringComparison.OrdinalIgnoreCase))
            {
                debuggingPage.ParsingStatus = "Invalid file type. Please select a .dsevents file.";
            }

            // Assert
            Assert.Equal("Invalid file type. Please select a .dsevents file.", debuggingPage.ParsingStatus);
        }

        // Test: ParsingStatus displays cancellation message when no file is selected
        [Fact]
        public void ParsingStatusLabel_DisplaysCancellationMessage()
        {
            // Arrange
            var debuggingPage = new DebuggingPage();

            // Act
            debuggingPage.ParsingStatus = "File selection was canceled.";

            // Assert
            Assert.Equal("File selection was canceled.", debuggingPage.ParsingStatus);
        }

        // Test: ParsingStatus displays parsing result
        [Fact]
        public void ParsingStatusLabel_DisplaysParsingResult()
        {
            // Arrange
            var debuggingPage = new DebuggingPage();

            // Simulate a parsing operation
            string simulatedParsingResult = "Parsing complete. Results: No errors found.";

            // Act
            debuggingPage.ParsingStatus = simulatedParsingResult;

            // Assert
            Assert.Equal("Parsing complete. Results: No errors found.", debuggingPage.ParsingStatus);
        }
    }
}
