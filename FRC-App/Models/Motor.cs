// Motor.cs
/*
 * Motor class:
 * - Attributes: _rpm, _torque, _temperature, _voltage
 * - Private attributes, public getters, and setters for real-time updates
 * - Methods to update all motor data
 */

public class Motor
{
    // Private attributes
    private double _rpm;         // Rotations Per Minute
    private double _torque;      // Torque in Nm
    private double _temperature; // Motor temperature in Celsius
    private double _voltage;     // Motor voltage in Volts

    // Constructor
    public Motor(double rpm, double torque, double temperature, double voltage)
    {
        _rpm = rpm;
        _torque = torque;
        _temperature = temperature;
        _voltage = voltage;
    }

    // Getters
    public double GetRPM() => _rpm;
    public double GetTorque() => _torque;
    public double GetTemperature() => _temperature;
    public double GetVoltage() => _voltage;

    // Real-time update methods
    public void UpdateRPM(double newRPM) { _rpm = newRPM; }
    public void UpdateTorque(double newTorque) { _torque = newTorque; }
    public void UpdateTemperature(double newTemp) { _temperature = newTemp; }
    public void UpdateVoltage(double newVoltage) { _voltage = newVoltage; }
