# CANServer Utils
A .NET utility for reading, decoding, and exporting binary CANServer logs in a fast and efficient way

## What?
CANServer is a fantastic hardware device used for snooping on multiple CAN bus networks through an ESP driven server. The developers also supply some handy scripts for converting the binary logs (performance optimised) into data that can be more easily read. 
https://github.com/joshwardell/CANserver

Enter `cantools`, a Python utility for decoding DBC files. DBC files act as both a reference, and archive of logged CAN data. The CANServer developers have also decoded a large and quite comprehensive list of messages and signals from the Tesla Model 3 network, and made it available in the DBC format
https://github.com/joshwardell/model3dbc

## Why?
Current tooling is driven in Python, which I have found to be adequate for small log generation. However, I am intending to download, decode, and export a large (likely continuous) stream of log data from the device installed permatently in my personal vehicle. For this, I have chosen a language and environment I am familiar with in the hopes of using lower level streaming techniques to increase performance of the incoming log translation on the way to at-rest storage (in my case, InfluxDB)

## How?
This tooling will likely evolve in two parts; CANServer Deserialisation, and DBC file parsing;

### CANServer Download Deserialisation
CANServer writes its logged CAN data in an optimised format to make the most of the limited performance and storage space available on the device. Therefore, a bespoke format is used to write packets into a stored log file, which is rotated on the device. These logs are then made available either on the SD Card after removal, or over HTTP interface through a WiFi connection.

### DBC File Parsing
In this project, DBC files are only used for definitions, and after research the limited number of DBC libraries available are intended to extract full logs.

Thus, I need to extract the DBC definitions, and then use them to parse the (now deserialised) CAN frames from the CANServer.

For this, Datalust Superpower is being used to develop a parser for DBC file definiions; it will tokenise, and parse the file into a managed object which will then be used to decode frames from an adjacent source
