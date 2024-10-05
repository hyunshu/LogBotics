using System;
using System.Collections.Generic;
using System.IO;

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
     * --- storeRawData() ---
     * Stores the inputted raw data to the disk as variables via SQL.
     * @param rawData
     * @param fileName
     */
    public void StoreRawData(List<List<List<double>>> rawData)
    {
        // TODO
    }

    public void SendRawData()
    {
        // TODO
    }
}
