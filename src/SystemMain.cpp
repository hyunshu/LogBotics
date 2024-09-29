//
// Created by james on 9/26/2024.
//

#include <iostream>
#include <vector>
#include "../inlcude/Tab.h"
#include "../inlcude/Account.h"
#include "../inlcude/DataImport.h"



/**
 * --- createHomepage() ---
 * Sets up GUI and related data for the LogBotics Homepage after login.
 */

static void createHomepage() {
    //TODO: Needs to be Implemented

    std::cout << "\ncreatHomepage Function Accessed" << std::endl;
} /* createHomepage() */



/**
 * --- generateTestImport() ---
 * Constructs the DataImport sample with dataTypes and labels populated with fake simulated values.
 * Function used for testing/debugging purposes.
 * @return DataImport
 */

static DataImport generateTestImport() {
    //Populate sample data types and labels:

    std::vector<std::string> dataTypes = {"Motor", "Sensor"};
    std::vector<std::string> motorLabels = {"Time (s)","Spin Angle (rad)","Angular Velocity (rad/s)"};
    std::vector<std::string> sensorLabels = {"Time (s)","Measurement #1 (ft)","Measurement #2 (rad)"};

    //Construct and return DataImport object:

    std::vector<std::vector<std::string>> dataUnits = {motorLabels, sensorLabels};
    DataImport testImport(dataTypes, dataUnits);
    return testImport;
} /* generateTestImport() */



/**
 * --- generateTestData() ---
 * Generates and formats fake FRC test data (rawData).
 * Function used for testing/debugging purposes.
 * @return std::vector<std::vector<std::vector<double>>>
 */

static std::vector<std::vector<std::vector<double>>> generateTestData() {
    //Data to be populated for both the Motor and Sensor:

    std::vector<double> time{};
    std::vector<double> SAData{};
    std::vector<double> AVData{};
    std::vector<double> firstSensorData{};
    std::vector<double> secondSensorData{};

    //Populate both the Motor and Sensor data with random values:

    int n = 10;  //Number of data steps used both files
    for (double i = 0; i < n; i++) {
        time.push_back(i);
        SAData.push_back(rand());
        AVData.push_back(rand());
        firstSensorData.push_back(rand());
        secondSensorData.push_back(rand());
    }

    //Format and return the resulting rawData:

    std::vector<std::vector<double>> rawMotorData = {time,SAData,AVData};
    std::vector<std::vector<double>> rawSensorData = {time,firstSensorData,secondSensorData};
    std::vector<std::vector<std::vector<double>>> rawData = {rawMotorData, rawSensorData};
    return rawData;
} /* generateTestData() */



int main() {
    //Define Fields:

    std::vector<Tab> tabs{};
    std::vector<Account> accounts{};
    std::vector<int> visualizationSettings{}; //Will probably want this to have its own class later
    std::vector<std::vector<std::vector<double>>> rawData{}; //3D vector (x,y data for z different sensors/motors)

    std::cout << "Field Declarations were Successful" << std::endl;

    //Generate LogBotics Homepage:

    createHomepage();

    //TESTING:

    //Generate Fake Test FRC Data:

    DataImport testImport = generateTestImport();
    rawData = generateTestData();

    //Test data storage schema:

    testImport.storeRawData(rawData, "SampleDemo");

    return 0;
}

