using System;
using System.Collections.Generic;
using System.IO;
using FRC_App.Models;
// using Windows.Globalization.NumberFormatting; // does not work in MacOS

public class DataImport
{
    public string sessionName;
    public List<string> dataTypes;
    public List<List<string>> dataUnits;

    /**
     * --- DataImport() #1 ---
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
        List<string> dataTypes = new List<string> { "Motor", "Sensor", "ControlSystem", "Accelerometer" };
        List<string> motorLabels = new List<string> { "Time (s)", "Spin Angle (rad)", "Angular Velocity (rad/s)" };
        List<string> sensorLabels = new List<string> { "Time (s)", "Measurement #1 (ft)", "Measurement #2 (rad)" };
        List<string> controlSystemLabels = new List<string> { "Time (s)", "Forward Input (bool)", "Backward Input (bool)" };
        List<string> accelerometerLabels = new List<string> { "Time (s)", "X-Acceleration (ft/s^2)", "Y-Acceleration (ft/s^2)", "Z-Acceleration (ft/s^2)" };

        // Construct and return DataImport object:
        List<List<string>> dataUnits = new List<List<string>> { motorLabels, sensorLabels, controlSystemLabels, accelerometerLabels };
        
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
        List<double> timeMotor = new List<double>();
        List<double> SAData = new List<double>();
        List<double> AVData = new List<double>();
        List<double> firstSensorData = new List<double>();
        List<double> secondSensorData = new List<double>();
        List<double> forwardInputData = new List<double>();
        List<double> backwardInputData = new List<double>();
        List<double> xAccelData = new List<double>();
        List<double> yAccelData = new List<double>();
        List<double> zAccelData = new List<double>();

        // Populate both the Motor and Sensor data with random values:
        int n = 10;  // Number of data steps used in both files
        Random rand = new Random(0);
        for (double i = 0; i < n; i++)
        {
            time.Add(i);
            timeMotor.Add(i);
            SAData.Add(rand.NextDouble());
            AVData.Add(rand.NextDouble());
            firstSensorData.Add(rand.NextDouble());
            secondSensorData.Add(rand.NextDouble());
            forwardInputData.Add(Math.Round(rand.NextDouble()));
            backwardInputData.Add(Math.Round(rand.NextDouble()));
            xAccelData.Add(rand.NextDouble());
            yAccelData.Add(rand.NextDouble());
            zAccelData.Add(rand.NextDouble());
        }
        timeMotor.Add(n);
        SAData.Add(rand.NextDouble());
        SAData[0] += 9999;

        // Format and return the resulting rawData:
        List<List<double>> rawMotorData = new List<List<double>> { timeMotor, SAData, AVData };
        List<List<double>> rawSensorData = new List<List<double>> { time, firstSensorData, secondSensorData };
        List<List<double>> rawControlSystemData = new List<List<double>> { time, forwardInputData, backwardInputData };
        List<List<double>> accelerometerData = new List<List<double>> { time, xAccelData, yAccelData, zAccelData };
        List<List<List<double>>> rawData = new List<List<List<double>>> { rawMotorData, rawSensorData, rawControlSystemData, accelerometerData };
        return rawData;
    }


    public List<List<List<double>>> FromRobot(string directoryPath, string fileName) {
        List<string> dataTypes = new List<string> {};
        List<List<string>> dataUnits = new List<List<string>> {};
        List<List<List<double>>> rawData = new List<List<List<double>>> {};

        var filestream = new FileStream(directoryPath + fileName,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.ReadWrite);
        var file = new StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);

        string lineOfText = "";
        while ((lineOfText = file.ReadLine()) != null)
        {
            if (lineOfText == "NT4 Client Connected" || lineOfText == "NT4 Client Disconnected") {
                continue;
            }
            if (lineOfText.Contains("(from stream):")) {
                continue;
            }
            if (lineOfText == "\n" || lineOfText == "") {
                break;
            }

            string dataType = lineOfText.Split(":",StringSplitOptions.RemoveEmptyEntries).First();
            if (!dataType.Any() || !dataTypes.Contains(dataType)) {
                dataTypes.Add(dataType);
            }

            int typeIndex = dataTypes.IndexOf(dataType);

            string dataUnit = lineOfText.Split(":",StringSplitOptions.RemoveEmptyEntries)[1];
            if (dataUnits.Count - 1 < typeIndex) {
                dataUnits.Add(new List<string>{});
            }
            if (!dataUnits[typeIndex].Any() || !dataUnits[typeIndex].Contains(dataUnit)) {
                dataUnits[typeIndex].Add(dataUnit);
            }

            int unitIndex = dataUnits[typeIndex].IndexOf(dataUnit);

            string data = lineOfText.Split(":",StringSplitOptions.RemoveEmptyEntries).Last();
            data = data.Substring(1); //remove the space
            double x = double.Parse(data);

            if (rawData.Count - 1 < typeIndex) {
                rawData.Add(new List<List<double>>{});
            }
            if (rawData[typeIndex].Count - 1 < unitIndex) {
                rawData[typeIndex].Add(new List<double>{});
            }

            rawData[typeIndex][unitIndex].Add(x); 
        }

        this.dataTypes = dataTypes;
        this.dataUnits = dataUnits;
        return rawData;
    }

    /**
     * --- FromCSV() ---
     * Reads the inputted raw data from the disk in a CSV under "fileName".
     * Breaks the csv into each type (motor, sensor, ...)
     * and labels as well to the various data streams into variables
     * @param rawData
     * @param fileName
     */
    public List<List<List<double>>> FromCSV(string directoryPath, string fileName)
    {
        //Testing Only need this ordering part to run the testcases:
        String[] oldFileNames = Directory.GetFiles(directoryPath); //Rename to fileNames if not ordering for test cases
        String[] fileNames = new String[4];

        List<string> dataTypesFormat = new List<string> { "Motor", "Sensor", "ControlSystem", "Accelerometer" };
        for (int i = 0; i < 4; i++) {
            foreach (string file2 in oldFileNames) {
                string readFileName2 = file2.Split("\\",StringSplitOptions.RemoveEmptyEntries).Last();
                string secondHalf = readFileName2.Split("_",StringSplitOptions.RemoveEmptyEntries).Last();
                int typeEnd = secondHalf.IndexOf(".csv", StringComparison.Ordinal);
                string dataType = secondHalf.Substring(0, typeEnd);
                if (dataTypesFormat[i].Equals(dataType)) {
                    fileNames[i] = file2;
                }
            }
        }
        //Testing Only need this ordering part to run the testcases

        List<string> dataTypes = new List<string> {};
        List<List<string>> dataUnits = new List<List<string>> {};
        List<List<List<double>>> rawData = new List<List<List<double>>> {};

        foreach (string file in fileNames)
        {
            string readFileName = file.Split("\\",StringSplitOptions.RemoveEmptyEntries).Last();
            int nameEnd = readFileName.IndexOf("_", StringComparison.Ordinal);
            if (fileName.Equals(readFileName.Split("_",StringSplitOptions.RemoveEmptyEntries).First())) {
                
                int typeEnd = readFileName.IndexOf(".csv", StringComparison.Ordinal);
                string dataType = readFileName.Substring(nameEnd + 1, typeEnd - nameEnd - 1);
                dataTypes.Add(dataType);

                using (StreamReader reader = new StreamReader(file)) 
                {
                    string labels = reader.ReadLine();
                    List<string> fileUnits = labels.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
                    dataUnits.Add(fileUnits);

                    String line = "";
                    List<List<double>> fileData = new List<List<double>>(fileUnits.Count());
                    for (int i=0;i<fileUnits.Count();i++) fileData.Add(new List<double>{});

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',',StringSplitOptions.RemoveEmptyEntries);
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

        this.dataTypes = dataTypes;
        this.dataUnits = dataUnits;
        return rawData;
    }

    /**
     * --- storeRawData() ---
     * Stores the inputted raw data to the disk as variables via SQL.
     * Does this by compacting the data into strings (simple variable)
     * that can be stored by SQL.
     * @param rawData
     * @param user
     * @param sessionName
     */
    public void StoreRawData(List<List<List<double>>> rawData, User user, string sessionName)
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

        if (string.IsNullOrEmpty(user.sessions)){
            user.sessions = sessionName;
            user.dataTypes = dataTypes;
            user.dataUnits = dataUnits;
            user.rawData = rawDataString;
        } else {
            user.sessions += "|" + sessionName;
            user.dataTypes += "|" + dataTypes;
            user.dataUnits += "|" + dataUnits;
            user.rawData += "|" + rawDataString;
        }
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
     * @param sessionName
     * @returns rawData
     */
    public List<List<List<double>>> RetrieveRawData(User user, string sessionName)
    {
        this.sessionName = sessionName;
        int sessionIndex = user.sessions.Split("|",StringSplitOptions.RemoveEmptyEntries).ToList().FindIndex(x => x.Equals(sessionName));
        string dataTypesString = user.dataTypes.Split("|",StringSplitOptions.RemoveEmptyEntries).ToList()[sessionIndex];
        string dataUnitsString = user.dataUnits.Split("|",StringSplitOptions.RemoveEmptyEntries).ToList()[sessionIndex];
        string rawDataString = user.rawData.Split("|",StringSplitOptions.RemoveEmptyEntries).ToList()[sessionIndex];


        this.dataTypes = dataTypesString.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();


        this.dataUnits.Clear();
        List<string> filesUnits = dataUnitsString.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (string file in filesUnits)
        {
            List<string> units = file.Split(";",StringSplitOptions.RemoveEmptyEntries).ToList();
            this.dataUnits.Add(units);
        }


        List<List<List<double>>> rawData = new List<List<List<double>>> {};
        List<string> filesData = rawDataString.Split("_",StringSplitOptions.RemoveEmptyEntries).ToList();
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