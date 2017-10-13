# NmeaGoodies
A C#/Windows toolkit for NMEA 0183 communications (TCP/serial), interpretation, construction and distribution of messages using RabbitMQ. Includes some instruments and a compass calibration tool

This is an unfinished project to be able to build Glass Cockpit instruments - gauges and the like. I also built this to calibrate a digital compass on a marine autopilot and refine the autopilots behavior.

All this uses RabbitMQ to store and forward messages. Most of it requires RabbitMQ to be installed. 

There's a couple different things here:

* Libraries
  * Geometry  - Earth coordinate math library. Includes methods for great circle navigation, distance and bearing
  * NMEA0183  - Receiving, Sending, Interpretation and Composition of NMEA 0183 messages using RabbitMQ Queues.
* Exchangers - these are the bits that communicate with devices and RabbitMQ exchanges. You need to have one or more running to have things communicate. It's also possible to use them to connect one to another.
  * TCPExchange - communicate on a tcp port
  * Serial Exchange - communicate on a COM port. Needed for USB or Bluetooth communications
  
* Applications
  * HelloWorld - This simply dumps all NMEA messages that come in on a RabbitMQ queue.
  * DepthGauge - Simple WPF app that shows current depth from depthsounder/fishfinder messages
  * ConsoleLog - This program prints all messages coming in on an exchange to the console.  Messages that we can interpret are yelolow, others in red.
* Experiments
  * No good documentation/frequently modified.