#include <iostream>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib") // Winsock Library

int main() {
    WSADATA wsa;
    SOCKET s;
    struct sockaddr_in server;

    // Initialize Winsock
    std::cout << "Initializing Winsock..." << std::endl;
    if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) {
        std::cerr << "Failed. Error Code: " << WSAGetLastError() << std::endl;
        return 1;
    }

    std::cout << "Initialized.\n";

    // Create a socket
    if ((s = socket(AF_INET, SOCK_STREAM, 0)) == INVALID_SOCKET) {
        std::cerr << "Could not create socket: " << WSAGetLastError() << std::endl;
        return 1;
    }

    std::cout << "Socket created.\n";

    server.sin_addr.s_addr = inet_addr("172.22.11.2");
    server.sin_family = AF_INET;
    server.sin_port = htons(5800);

    // Connect to the RoboRIO
    if (connect(s, (struct sockaddr *)&server, sizeof(server)) < 0) {
        std::cerr << "Connection error" << std::endl;
        return 1;
    }

    std::cout << "Connected to RoboRIO.\n";

    // Clean up
    closesocket(s);
    WSACleanup();

    return 0;
}
