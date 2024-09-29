//
// Created by james on 9/26/2024.
//

#include <string>
#include <vector>

#ifndef LOGBOTICS_ACCOUNT_H
#define LOGBOTICS_ACCOUNT_H

class Account {
    public:
        //Fields:
        std::string userName;
        std::string password;
        std::vector<int> settings;
        std::string teamName;
        int teamNumber;

        //Functions:
        static Account createAccount();
};

#endif //LOGBOTICS_ACCOUNT_H

