using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class DSEventParser
{
    public static List<string> ParseErrorsAndWarnings(string filePath)
    {
        List<string> errorsAndWarnings = new List<string>();
        string[] keywords = { "error", "warning" };
        string pattern = string.Join("|", keywords);

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
                {
                    errorsAndWarnings.Add(line);
                }
            }
        }

        return errorsAndWarnings;
    }
}
