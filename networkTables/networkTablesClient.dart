//STILL NEED TO INSTALL DART/FLUTTER

import 'dart:io';

import 'package:nt4/nt4.dart';
import 'dart:async';

void main() {
  final NT4Client nt4Client = NT4Client(serverAddr: '172.22.11.2'); //robot IP over USB
  StreamSubscription? subscription;

  void connectToNetworkTables() {
    // Connect to the NetworkTables server
    nt4Client.connect();

    // Delay a bit to ensure connection is established before listing keys
    Future.delayed(Duration(seconds: 2), () {
      listKeys(nt4Client);
    });
  }

  void listKeys(NT4Client nt4Client) {
    nt4Client.getKeys().then((keys) {
      for (String key in keys) {
        print('Available key: $key');
      }
    });
  }

  // Connect to NetworkTables
  connectToNetworkTables();

  // Disconnect on exit
  ProcessSignal.sigint.watch().listen((_) {
    subscription?.cancel();
    nt4Client.disconnect();
    print('Disconnected from NetworkTables');
    exit(0);
  });
}
