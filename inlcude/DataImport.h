//
// Created by james on 9/26/2024.
//

#include <vector>
#include <string>

#ifndef LOGBOTICS_DATAIMPORT_H
#define LOGBOTICS_DATAIMPORT_H

class DataImport {
    public:
        //Fields:

        std::vector<std::string> dataTypes{};  //ie. Motor, sensor, or control system
        std::vector<std::vector<std::string>> dataUnits{};

        //Constructor:

        DataImport(std::vector<std::string> dataTypes, std::vector<std::vector<std::string>> dataUnits);

        //Functions:

        void storeRawData(std::vector<std::vector<std::vector<double>>> rawData, std::string fileName);
        void sendRawData();
};

#endif //LOGBOTICS_DATAIMPORT_H
