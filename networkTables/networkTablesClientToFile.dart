//robot IP over usb: 172.22.11.2
//for this to work, user will have to make network table calls in their code using "robot" as the name
//of the NT
import 'dart:io';

import 'package:nt4/nt4.dart';
import 'dart:async';

void main() async {
  // Connect to NT4 server at your robot's IP address from usb
  String addr = '172.22.11.2';
  NT4Client client = NT4Client(
    serverBaseAddress: addr,
    onConnect: () {
      print('NT4 Client Connected');
    },
    onDisconnect: () {
      print('NT4 Client Disconnected');
    },
  );

  // Create a file to write the data with a timestamp in the user's documents directory
  String timestamp = DateTime.now().toIso8601String().replaceAll(':', '-');
  String userHome = Platform.environment['USERPROFILE'] ??
      Platform.environment['HOME'] ??
      '.';
  File dataFile = File(
      'C:/Users/Jenna/Documents/GitHub/LogBotics/FRC-App/robot_data_$timestamp.txt');
  IOSink fileSink = dataFile.openWrite(mode: FileMode.append);

  // Subscribe to motor output topic
  NT4Subscription motorOutputSub =
      client.subscribePeriodic('/robot/MotorOutput');

  // Subscribe to encoder values topic
  NT4Subscription encoderValuesSub =
      client.subscribePeriodic('/robot/EncoderOutput');

  // Subscribe to IMU position topic
  NT4Subscription pigeonPositionSub =
      client.subscribePeriodic('/robot/PigeonOutput');

  // Receive motor output data and write to file
  motorOutputSub.listen((data) {
    String logEntry = 'Motor Output: $data\n';
    fileSink.write(logEntry);
  });

  // Receive encoder values data and write to file
  encoderValuesSub.listen((data) {
    String logEntry = 'Encoder Values: $data\n';
    fileSink.write(logEntry);
  });

  // Receive IMU position data and write to file
  pigeonPositionSub.listen((data) {
    String logEntry = 'Pigeon Position: $data\n';
    fileSink.write(logEntry);
  });

  await for (Object? data in motorOutputSub.stream()) {
    String logEntry = 'Motor Output (from stream): $data\n';
    fileSink.write(logEntry);
  }

  await for (Object? data in encoderValuesSub.stream()) {
    String logEntry = 'Encoder Values (from stream): $data\n';
    fileSink.write(logEntry);
  }

  await for (Object? data in pigeonPositionSub.stream()) {
    String logEntry = 'Pigeon Position (from stream): $data\n';
    fileSink.write(logEntry);
  }

  // Close the file sink when done
  await fileSink.close();
}
