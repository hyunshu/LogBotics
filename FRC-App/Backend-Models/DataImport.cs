using System;
using System.Collections.Generic;
using System.IO;
using FRC_App.Models;

namespace FRC_App.Import
{
public class DataImport
{
    public List<string> dataTypes;
    public List<List<string>> dataUnits;

    /**
     * --- DataImport #1 ---
     * Constructs DataImport Object with its list of data types and
     * the data labels for each type.
     * @param dataTypes
     * @param dataUnits
     */
    public DataImport(List<string> dataTypes, List<List<string>> dataUnits)
    {
        this.dataTypes = dataTypes;
        this.dataUnits = dataUnits;
    }

    /**
     * --- DataImport() #2 ---
     * Constructs a DataImport sample with dataTypes and labels populated with fake simulated values.
     * Basically, if no data stucture (ie. labels) are specified, this constructor override populates 
     * it with fake values.
     * Constructor used for testing/debugging purposes.
     */
    public DataImport()
    {
        // Populate sample data types and labels:
        List<string> dataTypes = new List<string> { "Motor", "Sensor" };
        List<string> motorLabels = new List<string> { "Time (s)", "Spin Angle (rad)", "Angular Velocity (rad/s)" };
        List<string> sensorLabels = new List<string> { "Time (s)", "Measurement #1 (ft)", "Measurement #2 (rad)" };

        // Construct and return DataImport object:
        List<List<string>> dataUnits = new List<List<string>> { motorLabels, sensorLabels };
        
        this.dataTypes = dataTypes;
        this.dataUnits = dataUnits;
    }

	/**
     * --- generateTestData() ---
     * Generates and formats fake FRC test data (rawData).
     * Function used for testing/debugging purposes.
     * @return List<List<List<double>>>
     */
    public List<List<List<double>>> GenerateTestData()
    {
        // Data to be populated for both the Motor and Sensor:
        List<double> time = new List<double>();
        List<double> SAData = new List<double>();
        List<double> AVData = new List<double>();
        List<double> firstSensorData = new List<double>();
        List<double> secondSensorData = new List<double>();

        // Populate both the Motor and Sensor data with random values:
        int n = 10;  // Number of data steps used in both files
        Random rand = new Random();
        for (double i = 0; i < n; i++)
        {
            time.Add(i);
            SAData.Add(rand.NextDouble());
            AVData.Add(rand.NextDouble());
            firstSensorData.Add(rand.NextDouble());
            secondSensorData.Add(rand.NextDouble());
        }

        // Format and return the resulting rawData:
        List<List<double>> rawMotorData = new List<List<double>> { time, SAData, AVData };
        List<List<double>> rawSensorData = new List<List<double>> { time, firstSensorData, secondSensorData };
        List<List<List<double>>> rawData = new List<List<List<double>>> { rawMotorData, rawSensorData };
        return rawData;
    }

    /**
     * --- FromCSV() ---
     * Reads the inputted raw data from the disk in a CSV under "fileName".
     * Breaks the csv into each type (motor, sensor, ...)
     * and labels as well to the various data streams into variables
     * Should actually store raw data with SQL when that is implemented (not simply
     * return it).
     * @param rawData
     * @param fileName
     */
    public Tuple<DataImport,List<List<List<double>>>> FromCSV(string directoryPath, string fileName)
    {

        String[] fileNames = Directory.GetFiles($"../FRCData/");

        List<string> dataTypes = new List<string> {};
        List<List<string>> dataUnits = new List<List<string>> {};
        List<List<List<double>>> rawData = new List<List<List<double>>> {};

        foreach (string file in fileNames)
        {
            int nameEnd = file.IndexOf("_", StringComparison.Ordinal);

            if (String.Equals(file.Substring(0,nameEnd), fileName)) {
                
                int typeEnd = file.IndexOf(".csv", StringComparison.Ordinal);

                dataTypes.Append(file.Substring(nameEnd,typeEnd));

                using (StreamReader reader = new StreamReader($"{directoryPath}/{fileName}")) 
                {
                    string labels = reader.ReadLine();
                    List<string> fileUnits = labels.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
                    dataUnits.Append(fileUnits);

                    String line = "";
                    List<List<double>> fileData = new List<List<double>>(fileUnits.Count());

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        // Loop over each data step (row):
                        int j = 0;
                        foreach (var x in columns)
                        {
                            // Loop over each label (column):
                            double val = Double.Parse(x);
                            fileData[j].Append(val);
                            j++;
                        }
                    }
                    rawData.Append(fileData);
                }
            }
        }

        DataImport import = new DataImport(dataTypes,dataUnits);
        Tuple<DataImport, List<List<List<double>>>> allData = new Tuple<DataImport, List<List<List<double>>>>(import,rawData);
        return allData;
    }

    /**
     * --- storeRawData() ---
     * Stores the inputted raw data to the disk as variables via SQL.
     * @param rawData
     * @param fileName
     */
    public void StoreRawData(List<List<List<double>>> rawData, User user)
    {
        user.DataStructure = this;
        user.rawData = rawData;
    }

    public void SendRawData()
    {
        // TODO
    }
}
}
