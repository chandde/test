# Prerequisites
1.	Set up an ARM32 device with IoTCore 1809
2.	In your IoT Hub configuration on Azure Portal, add a new Edge device
3.	Once the device is created, set edgeHub and edgeAgent using these two modules,
`edgeshared.azurecr.io/microsoft/azureiotedge-agent:20190503.11-windows-arm32v7`
`edgeshared.azurecr.io/microsoft/azureiotedge-hub:20190503.11-windows-arm32v7`

4.	Also add two modules, the temperature sensor and iotedge-diagnostics, using below path
`edgeshared.azurecr.io/microsoft/azureiotedge-diagnostics:20190503.11-windows-arm32v7`
`edgeshared.azurecr.io/microsoft/azureiotedge-simulated-temperature-sensor:20190503.11-windows-arm32v7`

It's recommended to set the diagnostics module Restart Policy as Never, and Desired Status as Stopped, so the module will be deployed but never started. We'll run the module manually from remote ps-session and check its result locally, because the diagnostics module does not send the output to the hub.

Add credentials for the edgeshared container registry, username, password, and server, then submit the 4 modules

# Deploy and run iotedge
1.	Follow this link to enter remote ps-session for iotcore, https://docs.microsoft.com/en-us/windows/iot-core/connect-your-device/powershell
2.	In the ps-session, download the iotedge daemon set up script

`[10.137.198.225]: PS C:\Data> Invoke-WebRequest aka.ms/IotEdgeSecurityDaemonSetupScript -OutFile daemon.ps1`

3.	Import the downloaded script

`[10.137.198.225]: PS C:\Data> . .\daemon.ps1`

4.	Run function Deploy-IoTEdge, this function will download and install the latest cab for iotcore arm32, the device will reboot.
5.	After the package has been installed, enter the PS session again and import the script again
6.	Run function Initialize-IoTEdge, provided the device connection string for your iotedge device, also following the information for edgeAgent module

`[10.137.198.225]: PS C:\Data> Initialize-IoTEdge -DeviceConnectionString '<your device connection string>' -AgentImage 'edgeshared.azurecr.io/microsoft/azureiotedge-agent:20190503.11-windows-arm32v7' -Username "EdgeShared" -Password $(ConvertTo-SecureString '<real password>' -AsPlainText -Force)`

7.	Iotedge service should now pick up the edgeAgent module, run Get-IoTEdgeLog to see the service logs, also Get-Service IoTEdge to see the service status

## Run IoTEdge Check and other utils
Run iotedge check with the deployed iotedge-diagnostics module
```
[10.137.198.225]: PS C:\Data> iotedge check --diagnostics-image-name edgeshared.azurecr.io/microsoft/azureiotedge-diagnostics:20190503.11-windows-arm32v7 --verbose -c c:\data\programdata\iotedge\config.yaml                                                                 Configuration checks
--------------------
√ config.yaml is well-formed
√ config.yaml has well-formed connection string
√ container engine is installed and functional
√ config.yaml has correct hostname
√ config.yaml has correct URIs for daemon mgmt endpoint
‼ latest security daemon
    Installed IoT Edge daemon has version 1.0.8~dev20190503.3 but version 1.0.7 is available.
    Please see https://aka.ms/iotedge-update-runtime for update instructions.
‼ host time is close to real time
    Time on the device is out of sync with the NTP server. This may cause problems connecting to IoT Hub.
    Please ensure time on device is accurate, for example by setting up the Windows Time service to automatically sync with a time server.
√ container time is close to host time
‼ DNS server
    Container engine is not configured with DNS server setting, which may impact connectivity to IoT Hub.
    Please see https://aka.ms/iotedge-prod-checklist-dns for best practices.
    You can ignore this warning if you are setting DNS server per module in the Edge deployment.
        caused by: Could not open container engine config file C:\ProgramData\iotedge-moby\config\daemon.json
        caused by: The system cannot find the path specified. (os error 3)
‼ production readiness: certificates
    Device is using self-signed, automatically generated certs.
    Please see https://aka.ms/iotedge-prod-checklist-certs for best practices.
√ production readiness: certificates expiry
√ production readiness: container engine
‼ production readiness: logs policy
    Container engine is not configured to rotate module logs which may cause it run out of disk space.
    Please see https://aka.ms/iotedge-prod-checklist-logs for best practices.
    You can ignore this warning if you are setting log policy per module in the Edge deployment.
        caused by: Could not open container engine config file C:\ProgramData\iotedge-moby\config\daemon.json
        caused by: The system cannot find the path specified. (os error 3)

Connectivity checks
-------------------
√ host can connect to and perform TLS handshake with IoT Hub AMQP port
√ host can connect to and perform TLS handshake with IoT Hub HTTPS port
√ host can connect to and perform TLS handshake with IoT Hub MQTT port
√ container on the IoT Edge module network can connect to IoT Hub AMQP port
√ container on the IoT Edge module network can connect to IoT Hub HTTPS port
√ container on the IoT Edge module network can connect to IoT Hub MQTT port
√ Edge Hub can bind to ports on host

One or more checks raised warnings.
[10.137.198.225]: PS C:\Data>   
```

## IotEdge List
```
[10.137.198.225]: PS C:\Data> iotedge list                                                                                                                                                                                                                                     NAME             STATUS           DESCRIPTION      CONFIG
iotedge-diag     stopped          Stopped          edgeshared.azurecr.io/microsoft/azureiotedge-diagnostics:20190503.11-windows-arm32v7
edgeAgent        running          Up 33 minutes    edgeshared.azurecr.io/microsoft/azureiotedge-agent:20190503.11-windows-arm32v7
tempsensor       running          Up 21 minutes    edgeshared.azurecr.io/microsoft/azureiotedge-simulated-temperature-sensor:20190503.11-windows-arm32v7
edgeHub          running          Up 23 minutes    edgeshared.azurecr.io/microsoft/azureiotedge-hub:20190503.11-windows-arm32v7
```

# Appendix

## Sample IotEdge Logs output for edgeAgent
```
[10.137.198.225]: PS C:\Data> iotedge logs edgeAgent --tail 20                                                                                                                                                                                                                 <6> 2019-05-07 16:50:45.976 -07:00 [INF] - Executing command: "Command Group: (\n  [Stop module edgeHub]\n  [Start module edgeHub]\n  [Saving edgeHub to store]\n)"
<6> 2019-05-07 16:50:45.977 -07:00 [INF] - Executing command: "Stop module edgeHub"
<6> 2019-05-07 16:50:46.195 -07:00 [INF] - Executing command: "Start module edgeHub"
<6> 2019-05-07 16:50:54.021 -07:00 [INF] - Executing command: "Saving edgeHub to store"
<6> 2019-05-07 16:50:54.045 -07:00 [INF] - Plan execution ended for deployment 5
<6> 2019-05-07 16:50:54.897 -07:00 [INF] - Updated reported properties
<6> 2019-05-07 16:51:00.283 -07:00 [INF] - Updated reported properties
<6> 2019-05-07 16:53:20.488 -07:00 [INF] - Plan execution started for deployment 5
<6> 2019-05-07 16:53:20.488 -07:00 [INF] - Executing command: "Command Group: (\n  [Stop module tempsensor]\n  [Start module tempsensor]\n  [Saving tempsensor to store]\n)"
<6> 2019-05-07 16:53:20.489 -07:00 [INF] - Executing command: "Stop module tempsensor"
<6> 2019-05-07 16:53:20.511 -07:00 [INF] - Executing command: "Start module tempsensor"
<6> 2019-05-07 16:53:24.846 -07:00 [INF] - Executing command: "Saving tempsensor to store"
<6> 2019-05-07 16:53:24.850 -07:00 [INF] - Plan execution ended for deployment 5
<6> 2019-05-07 16:53:25.204 -07:00 [INF] - Updated reported properties
<6> 2019-05-07 16:53:30.333 -07:00 [INF] - HealthRestartPlanner is clearing restart stats for module 'tempsensor' as it has been running healthy for 00:10:00.
<6> 2019-05-07 16:53:30.334 -07:00 [INF] - Plan execution started for deployment 5
<6> 2019-05-07 16:53:30.337 -07:00 [INF] - Executing command: "Saving tempsensor to store"
<6> 2019-05-07 16:53:30.378 -07:00 [INF] - Plan execution ended for deployment 5
<6> 2019-05-07 16:53:30.576 -07:00 [INF] - Updated reported properties
<6> 2019-05-07 16:53:35.862 -07:00 [INF] - Updated reported properties
```
## Sample IotEdge logs output for temp sensor module
```
[10.137.198.225]: PS C:\Data> iotedge logs tempsensor --tail 5                                                                                                                                                                                                                         5/7/2019 4:57:15 PM> Sending message: 39, Body: [{"machine":{"temperature":41.311389315203471,"pressure":3.3139557447700154},"ambient":{"temperature":21.159575671264705,"humidity":25},"timeCreated":"2019-05-07T23:57:15.8722238Z"}]
        5/7/2019 4:57:20 PM> Sending message: 40, Body: [{"machine":{"temperature":41.75127328897419,"pressure":3.3640691088704773},"ambient":{"temperature":21.008210290460013,"humidity":26},"timeCreated":"2019-05-07T23:57:20.9173357Z"}]
        5/7/2019 4:57:26 PM> Sending message: 41, Body: [{"machine":{"temperature":42.802154434403469,"pressure":3.4837897456915345},"ambient":{"temperature":20.612583230302011,"humidity":25},"timeCreated":"2019-05-07T23:57:26.0718461Z"}]
        5/7/2019 4:57:31 PM> Sending message: 42, Body: [{"machine":{"temperature":42.955761865226435,"pressure":3.5012893264182012},"ambient":{"temperature":21.375939590798662,"humidity":26},"timeCreated":"2019-05-07T23:57:31.1088409Z"}]
        5/7/2019 4:57:36 PM> Sending message: 43, Body: [{"machine":{"temperature":43.996604232232364,"pressure":3.6198663049378643},"ambient":{"temperature":21.054292581302249,"humidity":24},"timeCreated":"2019-05-07T23:57:36.2057992Z"}]
```

## Use device explorer to view the data sent from edge device to the hub
1. Get the hub connection string, go to the hub portal, shared access policies, iothubowner, you'll see the hub connection string on the right side popup
![HubConnectionString](./HubConnectionString.png)
2. Install iot hub device explorer from https://github.com/Azure/azure-iot-sdk-csharp/releases/download/2019-1-4/SetupDeviceExplorer.msi, then launch device explorer
3. Fill in the hub connection string and click update
4. Switch to Data tab, select the device, if temp sensor is running well, you'll see data output as below
![TempSensorData](./DeviceExplorerTempSensorData.png)
