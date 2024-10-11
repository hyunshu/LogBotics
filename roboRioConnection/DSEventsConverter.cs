using System;
using System.Collections.Generic;
using System.IO;

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
                    if (line.Contains("<TagVersion>"))
                    {
                        tagVersion = ExtractValue(line, "<TagVersion>", " ");
                    }
                    if (line.Contains("<time>"))
                    {
                        time = ExtractValue(line, "<time>", " ");
                    }
                    if (line.Contains("<count>"))
                    {
                        count = ExtractValue(line, "<count>", " ");
                    }
                    if (line.Contains("<flags>"))
                    {
                        flags = ExtractValue(line, "<flags>", " ");
                    }
                    if (line.Contains("<Code>"))
                    {
                        code = ExtractValue(line, "<Code>", " ");
                    }
                    if (line.Contains("<details>"))
                    {
                        details = ExtractValue(line, "<details>", "<location>");
                    }
                    if (line.Contains("<location>"))
                    {
                        location = ExtractValue(line, "<location>", "<stack>");
                    }
                    if (line.Contains("<message>"))
                    {
                        message = ExtractValue(line, "<message>", " ");
                    }

                    // Add row when all fields are populated
                    if (!string.IsNullOrEmpty(tagVersion) && !string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(count))
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