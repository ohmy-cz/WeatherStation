
# Philips HUE Weather Station
Home weather station using Philips HUE [indoor](https://www2.meethue.com/en-us/p/hue-motion-sensor/046677473389) and [outdoor](https://www2.meethue.com/en-us/p/hue-outdoor-sensor/046677541736) sensors

## The project
Not a lot of Philips HUE owners know that the motion sensors can do more than simply detect motion and be a tiny part in the smart home chain. These sensors also measure *temperature*, humidity, and ambient light.

This project focuses on periodically collecting temperatures from all your sensors, and presenting them with a nice, user-friendly interactive real-time chart, together with aggregated historical data.

This is made possible by using a cheap Raspberry PI 4, connected to your home network over wifi. This little computer will host a web URL that will allow you to access the application interface from *any* computer or phone on your home network, or from *anywhere* in the world (**if** you choose to do so by opening your Raspberry PI's IP address to the world in your wifi router).

## Online demo
[See here](https://weather.jancerveny.net) 

## Installation
This project has been designed to run on **Raspberry PI 4 with 4GB RAM**, running Raspbiann headless.

1. [Install .NET Core 3.1](https://edi.wang/post/2019/9/29/setup-net-core-30-runtime-and-sdk-on-raspberry-pi-4) runtime on your Raspberry.
2. Download this source code with GIT in Visual Studio, and publish both `net.jancerveny.weatherstation.Web` and `net.jancerveny.weatherstation.WorkerService` as Deployment Mode: *Framework-Dependend*, and Target Runtime: *Portable*
3. Connect to your Raspberry PI using SSH
4. Create two new services: one to host the website, and another to collect the data periodically:
`sudo nano /etc/sysystemd/system/weather-web.service`
Now add these lines in the text editor, and close by pressing `CTRL+X` and confirming with `Y`: 
```
[Unit]
Description=ASP.NET Core 3.1 App - Web

[Service]
WorkingDirectory=/htdocs/netcore/_tmp
ExecStart=/home/pi/dotnet-arm32/dotnet /htdocs/netcore/_index/CodeIndex.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-codeindex
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target

```

and a second service, to collect data:
`sudo nano /etc/sysystemd/system/weather-worker.service`
```
[Unit]
Description=ASP.NET Core 3.1 Weather Service Worker
After=postgresql.service

[Service]
WorkingDirectory=/etc/netcore-services/_tmp
ExecStart=/home/pi/dotnet-arm32/dotnet /etc/netcore-services/weather/net.jancer$
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-codeindex
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```