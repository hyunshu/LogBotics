//
// Created by james on 9/26/2024.
//

#include <string>

#ifndef LOGBOTICS_TAB_H
#define LOGBOTICS_TAB_H

class Tab {
    public:
        //Fields:
        //Need to implement Tiles!
        std::string tabLabel;
        std::pair<int,int> position;  //in pixels
        std::pair<int,int> dimensions;  //in pixels
};

#endif //LOGBOTICS_TAB_H
