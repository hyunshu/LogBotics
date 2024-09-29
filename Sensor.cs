// Sensor.cs
/*
 * Sensor class:
 * - Attributes: _sensorType, _sensorValue, _unit, _status
 * - Private attributes, public getters, and setters for real-time updates
 * - Methods to update all sensor data
 */

public class Sensor
{
    // Private attributes
    private string _sensorType;  // Type of sensor (e.g., temperature, pressure)
    private double _sensorValue; // Value from the sensor
    private string _unit;        // Unit of measurement (e.g., Celsius, Pascal)
    private bool _status;        // Sensor operational status

    // Constructor
    public Sensor(string sensorType, double sensorValue, string unit, bool status)
    {
        _sensorType = sensorType;
        _sensorValue = sensorValue;
        _unit = unit;
        _status = status;
    }

    // Getters
    public string GetSensorType() => _sensorType;
    public double GetSensorValue() => _sensorValue;
    public string GetUnit() => _unit;
    public bool GetStatus() => _status;

    // Real-time update methods
    public void UpdateSensorValue(double newValue) { _sensorValue = newValue; }
    public void UpdateStatus(bool newStatus) { _status = newStatus; }
    public void UpdateUnit(string newUnit) { _unit = newUnit; }
}
