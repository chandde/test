Prerequisites
1.	Set up an ARM32 device with IoTCore 1809
2.	In your IoT Hub configuration on Azure Portal, add a new Edge device
3.	Once the device is created, set edgeHub and edgeAgent using these two modules,
`edgeshared.azurecr.io/microsoft/azureiotedge-agent:20190503.11-windows-arm32v7`
`edgeshared.azurecr.io/microsoft/azureiotedge-hub:20190503.11-windows-arm32v7`

4.	Also add two modules, the temperature sensor and iotedge-diagnostics, using below path
`edgeshared.azurecr.io/microsoft/azureiotedge-diagnostics:20190503.11-windows-arm32v7`
`edgeshared.azurecr.io/microsoft/azureiotedge-simulated-temperature-sensor:20190503.11-windows-arm32v7`

Add credentials for the edgeshared container registry, <username>, <password>, and <server>, then submit the 4 modules

Deploy and initialize iotedge
1.	Follow this link to enter remote ps-session for iotcore, https://docs.microsoft.com/en-us/windows/iot-core/connect-your-device/powershell
2.	In the ps-session, download the iotedge daemon set up script
`[10.137.198.225]: PS C:\Data> Invoke-WebRequest aka.ms/IotEdgeSecurityDaemonSetupScript -OutFile 1.ps1`

3.	Import the downloaded script
`[10.137.198.225]: PS C:\Data> . .\1.ps1`

4.	Run function Deploy-IoTEdge, this function will download and install the latest cab for iotcore arm32, the device will reboot.
5.	After the package has been installed, enter the PS session again and import the script again
6.	Run function Initialize-IoTEdge, provided the device connection string for your iotedge device, also following the information for edgeAgent module
`[10.137.198.225]: PS C:\Data> Initialize-IoTEdge -DeviceConnectionString '<your device connection string>' -AgentImage 'edgeshared.azurecr.io/microsoft/azureiotedge-agent:20190503.11-windows-arm32v7' -Username "EdgeShared" -Password $(ConvertTo-SecureString '<real password>' -AsPlainText -Force)`

7.	Iotedge service should now pick up the edgeAgent module, run Get-IoTEdgeLog to see the service logs, also Get-Service IoTEdge to see the service status
