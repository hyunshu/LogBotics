//
// Created by james on 9/26/2024.
//

#include <iostream>
#include <vector>
#include "../inlcude/Tab.h"
#include "../inlcude/Account.h"

int main() {

    //Define Fields:
    std::vector<Tab> tabs{};
    std::vector<Account> accounts{};
    std::vector<int> visualizationSettings{}; //Will probably want this to have its own class later
    std::vector<std::vector<std::vector<int>>> rawData{}; //3D vector (x,y data for z different sensors/motors)

    std::cout << "Field Declarations were Successful" << std::endl;
    return 0;
}
