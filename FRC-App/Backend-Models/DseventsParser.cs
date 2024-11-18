using System.Text;
using System.Text.RegularExpressions;

namespace FRC_App.Utilities
{
    public static class DseventsParser
    {
        public static string Parse(string fileContent)
        {
            var output = new StringBuilder();

            // Define regex patterns for errors and warnings
            string errorPattern = @"(?i)(error|failed|lost communication)";
            string warningPattern = @"(?i)(warning)";

            // Split the content into lines
            string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            bool appendNextLine = false;
            string currentOutput = string.Empty;

            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, errorPattern))
                {
                    if (appendNextLine)
                    {
                        output.AppendLine(currentOutput);
                        appendNextLine = false;
                    }
                    currentOutput = $"<span style=\"color:red;\">Error:</span> {ExtractEssentialInfo(line)}";
                    appendNextLine = true;
                }
                else if (Regex.IsMatch(line, warningPattern))
                {
                    if (appendNextLine)
                    {
                        output.AppendLine(currentOutput);
                        appendNextLine = false;
                    }
                    currentOutput = $"<span style=\"color:darkorange;\">Warning:</span> {ExtractEssentialInfo(line)}";
                    appendNextLine = true;
                }
                else if (appendNextLine)
                {
                    // Add details to the current output
                    currentOutput += $"<br>▼ Details<br>{ExtractEssentialInfo(line)}";
                }
            }

            // Append any remaining output
            if (appendNextLine)
            {
                output.AppendLine(currentOutput);
            }

            return output.Length > 0 ? output.ToString() : "No errors or warnings found.";
        }

        private static string ExtractEssentialInfo(string line)
        {
            // Extracts only the essential parts of the message for better readability
            var match = Regex.Match(line, @"<details>(.*?)<");
            return match.Success ? match.Groups[1].Value : line;
        }
    }
}
