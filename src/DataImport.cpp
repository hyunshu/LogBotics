//
// Created by james on 9/26/2024.
//

#include <fstream>
#include "../inlcude/DataImport.h"



/**
 * --- DataImport ---
 * Constructs DataImport Object with its list of data types and
 * the data labels for each type.
 * @param dataTypes
 * @param dataUnits
 */

DataImport::DataImport(std::vector<std::string> dataTypes, std::vector<std::vector<std::string>> dataUnits) {
    this->dataTypes = dataTypes;
    this->dataUnits = dataUnits;
} /* DataImport() */



/**
 * --- storeRawData() ---
 * Stores the inputted raw data to the disk under "fileName" inside the LogBotics App.
 * Breaks the raw data into different csv files for each type (motor, sensor, ...)
 * and attaches labels to the various data streams in the csv file.
 * @param rawData
 * @param fileName
 */

void DataImport::storeRawData(std::vector<std::vector<std::vector<double>>> rawData, std::string fileName) {
    int i = 0;  //Iterator for the ith csv file
    for (std::string dataType : dataTypes) {
        //Loop through each dataType creating a new csv file each time:

        //Create and label a new CSV file:

        std::ofstream inFile;
        inFile.open("../FRCData/" + fileName + "_" + dataType + ".csv");

        //Write the data labels/units as the first row for the ith CSV:

        std::vector<std::string> dataFileUnits = dataUnits.at(i);
        for (std::string label : dataFileUnits) {
            inFile << label + ",";  //Headings for file
        }
        inFile << std::endl;

        //Write the raw data streams for the ith CSV

        std::vector<std::vector<double>> rawFileData = rawData.at(i);
        int n = rawFileData.at(0).size();  //Number of data steps used in this file
        for (int j = 0; j < n; j++) {
            //Loop over each data step (row):

            for (std::vector<double> y: rawFileData) {
                //Loop over each label (column):

                double x = y.at(j);
                inFile << x << ",";
            }
            inFile << "\n";
        }

        // Close out this CSV file:

        i++;
        inFile.close();
    }
} /* storeRawData() */



void DataImport::sendRawData() {
    //TODO
}
