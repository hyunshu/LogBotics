using System;
using System.Collections.Generic;
using System.IO;
using FRC_App.Models;
using Windows.Globalization.NumberFormatting;

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
        List<string> dataTypes = new List<string> { "Motor", "Sensor", "ControlSystem" };
        List<string> motorLabels = new List<string> { "Time (s)", "Spin Angle (rad)", "Angular Velocity (rad/s)" };
        List<string> sensorLabels = new List<string> { "Time (s)", "Measurement #1 (ft)", "Measurement #2 (rad)" };
        List<string> controlSystemLabels = new List<string> { "Time (s)", "Forward Input (bool)", "Backward Input (bool)" };

        // Construct and return DataImport object:
        List<List<string>> dataUnits = new List<List<string>> { motorLabels, sensorLabels, controlSystemLabels };
        
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
        List<double> forwardInputData = new List<double>();
        List<double> backwardInputData = new List<double>();

        // Populate both the Motor and Sensor data with random values:
        int n = 10;  // Number of data steps used in both files
        Random rand = new Random(0);
        for (double i = 0; i < n; i++)
        {
            time.Add(i);
            SAData.Add(rand.NextDouble());
            AVData.Add(rand.NextDouble());
            firstSensorData.Add(rand.NextDouble());
            secondSensorData.Add(rand.NextDouble());
            forwardInputData.Add(Math.Round(rand.NextDouble()));
            backwardInputData.Add(Math.Round(rand.NextDouble()));
        }

        // Format and return the resulting rawData:
        List<List<double>> rawMotorData = new List<List<double>> { time, SAData, AVData };
        List<List<double>> rawSensorData = new List<List<double>> { time, firstSensorData, secondSensorData };
        List<List<double>> rawControlSystemData = new List<List<double>> { time, forwardInputData, backwardInputData };
        List<List<List<double>>> rawData = new List<List<List<double>>> { rawMotorData, rawSensorData, rawControlSystemData };
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

                dataTypes.Add(file.Substring(nameEnd,typeEnd));

                using (StreamReader reader = new StreamReader($"{directoryPath}/{fileName}")) 
                {
                    string labels = reader.ReadLine();
                    List<string> fileUnits = labels.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
                    dataUnits.Add(fileUnits);

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
                            fileData[j].Add(val);
                            j++;
                        }
                    }
                    rawData.Add(fileData);
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
     * Does this by compacting the data into strings (simple variable)
     * that can be stored by SQL.
     * @param rawData
     * @param user
     */
    public void StoreRawData(List<List<List<double>>> rawData, User user)
    {
        string dataTypes = "";
        foreach (string type in this.dataTypes) 
        {
            dataTypes += type + "_";
        }
        dataTypes = dataTypes.Substring(0, dataTypes.Length - 1);


        string dataUnits = "";
        foreach (List<string> fileUnits in this.dataUnits) 
        {
            string fileUnitsString = "";
            foreach (string unit in fileUnits)
            {
                fileUnitsString += unit + ";";
            }
            dataUnits += fileUnitsString.Substring(0, fileUnitsString.Length - 1) + "_";
        }
        dataUnits = dataUnits.Substring(0, dataUnits.Length - 1);


        string rawDataString = "";
        foreach (List<List<double>> fileData in rawData)
        {
            string fileDataString = "";
            foreach (List<double> columnData in fileData)
            {
                string columnDataString = "";
                foreach (double data in columnData)
                {
                    columnDataString += data + ",";
                }
                fileDataString += columnDataString.Substring(0, columnDataString.Length - 1) + ";";
            }
            rawDataString += fileDataString.Substring(0, fileDataString.Length - 1) + "_";
        }
        rawDataString = rawDataString.Substring(0, rawDataString.Length - 1);


        user.dataTypes = dataTypes;
        user.dataUnits = dataUnits;
        user.rawData = rawDataString;
    }



    /**
     * --- retrieveRawData() ---
     * Retrieves the raw data stored on the disk under the user via SQL.
     * Does this by decompressing the data from strings (simple variable)
     * that can be stored by SQL back into variables for use in the app.
     * In addition to returning the raw data it also constructs this ImportData
     * object to match the corresponding data structure (dataTypes & dataUnits)
     * that was retrieved.
     * @param user
     * @returns rawData
     */
    public List<List<List<double>>> RetrieveRawData(User user)
    {
        this.dataTypes = user.dataTypes.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();


        this.dataUnits.Clear();
        List<string> filesUnits = user.dataUnits.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (string file in filesUnits)
        {
            List<string> units = file.Split(";",StringSplitOptions.RemoveEmptyEntries).ToList();
            this.dataUnits.Add(units);
        }


        List<List<List<double>>> rawData = new List<List<List<double>>> {};
        List<string> filesData = user.rawData.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (string file in filesData)
        {
            List<List<double>> z = new List<List<double>> {};
            List<string> columnsData = file.Split(";",StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string column in columnsData)
            {
                List<double> y = new List<double> {};
                List<string> rowsData = column.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var x in rowsData) 
                {
                    y.Add(Double.Parse(x));
                }
                z.Add(y);
            }
            rawData.Add(z);
        }
        return rawData;
    }

    public void SendRawData()
    {
        // TODO
    }
}
