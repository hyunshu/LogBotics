using System;
using System.Collections.Generic;
using System.IO;

public class DataExport
{
    private List<string> dataTypes;
    private List<List<string>> dataUnits;

    /**
     * --- DataExport ---
     * Constructs DataExport Object with its list of data types and
     * the data labels for each type (dataStucture which is based on the imported data).
     * @param dataTypes
     * @param dataUnits
     */
    public DataExport(DataImport dataStructure)
    {
        this.dataTypes = dataStructure.dataTypes;
        this.dataUnits = dataStructure.dataUnits;
    }

    /**
     * --- ToCSV() ---
     * Exports the inputted raw data from the disk to a CSV under "fileName".
     * Breaks the raw data into different CSV files for each type (motor, sensor, ...)
     * and attaches labels to the various data streams in the CSV file.
     * @param rawData
     * @param fileName
     */
    public void ToCSV(List<List<List<double>>> rawData, string fileName, string directoryPath)
    {
        int i = 0;  // Iterator for the ith CSV file
        foreach (var dataType in dataTypes)
        {
            // Loop through each dataType creating a new CSV file each time:

            // Create and label a new CSV file:
            using (StreamWriter writer = new StreamWriter($"{directoryPath}/{fileName}_{dataType}.csv"))
            {
                // Write the data labels/units as the first row for the ith CSV:
                var dataFileUnits = dataUnits[i];
                foreach (var label in dataFileUnits)
                {
                    writer.Write(label + ",");  // Headings for file
                }
                writer.WriteLine();

                // Write the raw data streams for the ith CSV:
                var rawFileData = rawData[i];
                int n = rawFileData[0].Count;  // Number of data steps used in this file
                for (int k = 0; k < rawFileData.Count; k++) {
                    if (rawFileData[k].Count > n) {
                        n = rawFileData[k].Count;
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    // Loop over each data step (row):
                    foreach (var y in rawFileData)
                    {
                        if (j >= y.Count) {
                            continue;
                        }
                        // Loop over each label (column):
                        double x = y[j];
                        writer.Write(x + ",");
                    }
                    writer.WriteLine();
                }
            }

            // Close out this CSV file:
            i++;
        }
    }

}
