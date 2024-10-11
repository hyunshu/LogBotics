using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FRC_App
{
    public class DSEventsConverter
    {
        public static void ConvertToCsv(string inputFilePath, string outputFilePath)
        {
            try
            {
                // Read all lines from the input file
                string[] lines = File.ReadAllLines(inputFilePath);

                // Create a list to store parsed rows
                List<string[]> rows = new List<string[]>();

                // Add header row to the list
                rows.Add(new string[] { "TagVersion", "Time", "Count", "Flags", "Code", "Details", "Location", "Message" });

                // Parse the contents of the file
                string tagVersion = "";
                string time = "";
                string count = "";
                string flags = "";
                string code = "";
                string details = "";
                string location = "";
                string message = "";

                foreach (var line in lines)
                {
                    // Use regular expressions to extract values between tags
                    tagVersion = ExtractValue(line, "<TagVersion>", " ");
                    time = ExtractValue(line, "<time>", " ");
                    count = ExtractValue(line, "<count>", " ");
                    flags = ExtractValue(line, "<flags>", " ");
                    code = ExtractValue(line, "<Code>", " ");
                    details = ExtractValue(line, "<details>", "<");
                    location = ExtractValue(line, "<location>", "<");
                    message = ExtractValue(line, "<message>", " ");

                    // Add row when at least one field is populated
                    if (!string.IsNullOrEmpty(tagVersion) || !string.IsNullOrEmpty(time) || !string.IsNullOrEmpty(count) ||
                        !string.IsNullOrEmpty(flags) || !string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(details) ||
                        !string.IsNullOrEmpty(location) || !string.IsNullOrEmpty(message))
                    {
                        rows.Add(new string[] { tagVersion, time, count, flags, code, details, location, message });

                        // Reset variables for the next record
                        tagVersion = time = count = flags = code = details = location = message = "";
                    }
                }

                // Write to CSV
                using (StreamWriter sw = new StreamWriter(outputFilePath))
                {
                    foreach (var row in rows)
                    {
                        sw.WriteLine(string.Join(",", row));
                    }
                }

                Console.WriteLine($"Data successfully converted to CSV: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Helper function to extract value between tags
        public static string ExtractValue(string line, string startTag, string endTag)
        {
            int startIndex = line.IndexOf(startTag) + startTag.Length;
            if (startIndex < startTag.Length) return ""; // Tag not found

            int endIndex = line.IndexOf(endTag, startIndex);
            if (endIndex == -1) endIndex = line.Length;

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DSEventsConverter <inputFilePath> <outputFilePath>");
                return;
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            ConvertToCsv(inputFilePath, outputFilePath);
        }
    }
}