//robot IP over usb: 172.22.11.2
//for this to work, user will have to make network table calls in their code using "robot" as the name
//of the NT
import 'dart:io';

import 'package:nt4/nt4.dart';
import 'dart:async';
import 'package:nt4/nt4.dart';

void main() async {
  // Connect to NT4 server at your robot's IP address from usb which should be constant
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

  // Subscribe to motor output topic
  NT4Subscription motorOutputSub = client.subscribePeriodic('/robot/MotorOutput');

  // Subscribe to encoder values topic
  NT4Subscription encoderValuesSub = client.subscribePeriodic('/robot/EncoderValues');

  // Subscribe to IMU position topic
  NT4Subscription pigeonPositionSub = client.subscribePeriodic('/robot/PigeonPosition');

  // Receive motor output data
  motorOutputSub.listen((data) => print('Motor Output: $data'));

  // Receive encoder values data
  encoderValuesSub.listen((data) => print('Encoder Values: $data'));

  // Receive IMU position data
  pigeonPositionSub.listen((data) => print('Pigeon Position: $data'));

  await for (Object? data in motorOutputSub.stream()) {
    print('Motor Output (from stream): $data');
  }

  await for (Object? data in encoderValuesSub.stream()) {
    print('Encoder Values (from stream): $data');
  }

  await for (Object? data in pigeonPositionSub.stream()) {
    print('Pigeon Position (from stream): $data');
  }
}
