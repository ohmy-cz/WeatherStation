
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

### Required knowledge
You need to know some basic knowledge about Visual Studio, networking, and Linux.

### Required tooling
I'm using Windows in this tutorial, but you should be able to connect from any platform. You will also need Visual Studio and [.NET Core 3.1 SDK installed](https://dotnet.microsoft.com/download), and then install [Entity Framework Core tools](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet), this will  help us create the local database.

### Install dependencies first
1. Get [Putty](https://www.putty.org/) for connecting to your Raspberry PI
2. [Install .NET Core 3.1](https://edi.wang/post/2019/9/29/setup-net-core-30-runtime-and-sdk-on-raspberry-pi-4) runtime on your Raspberry.
3. Install PostgreSQL on your Raspberry: use Putty to log in, then run `apt-get install postgresql-11`. Note down the username and password. For the tech savvy: thanks to using Entity Framework, you should be easily able to replace PostgrSQL with any other database, simply by removing PostgreSQL Nuget packages and dropping in MySQL, MicrosoftSQL, or other database provider in.
4. Create a `weather` database using *pgAdmin* for PostgreSQL

### Setup
1. Download this source code with GIT in Visual Studio
2. Update the appsettings.json in both `net.jancerveny.weatherstation.Web` and `net.jancerveny.weatherstation.WorkerService`, and replace the `<REPLACE>` strings with your own to reflect your local Philips HUE bridge and IP Address. 

TODO: Describe how to generate Philips HUE API key. For now, see [this link]([Install .NET Core 3.1](https://edi.wang/post/2019/9/29/setup-net-core-30-runtime-and-sdk-on-raspberry-pi-4) ) and temporarily overwrite `HomeController.cs`' `Index` method to generate it for you. 
3. Fill in the `ConnectionStrings` for your Raspberry PI PostgreSQL host in *both projects'* `appsettings.json` - replace the username and password parameters with your own database username and password. Then follow [this tutorial](http://www.postgresonline.com/journal/archives/38-PuTTY-for-SSH-Tunneling-to-PostgreSQL-Server.html) to set up a Putty SSH tunnel to allow your computer during build to connect to your Raspberry PI's new PostgreSQL database. Just follow the pictures. No worries, nothing needs to permanently run on your computer - this is a one-time action only.
4. Make sure Putty with a SSH tunnel set up is running. Now open *View/Other Windows/Package Manager Console* panel in VisualStudio. Enter this command: `Update-Database -s "net.jancerveny.weatherstation.Client.Web" -p "net.jancerveny.weatherstation.DataLayer" -c WeatherDbContext`. This should update your Raspberry PI database and create all required tables.
5. Publish *both projects* as Deployment Mode: *Framework-Dependend*, and Target Runtime: *Portable*
6. Using Filezilla, connect to your Raspberry PI (using ssh in the connection settings), and create/copy published projects in `/htdocs/netcore/weather/` and `/etc/netcore-services/weather` respectively. (You can choose differrent locations, as long as you will remember to change the paths in scripts below)
7. Connect to your Raspberry PI using Putty
8. Create two new services: one to host the website, and another to collect the data periodically:
`sudo nano /etc/sysystemd/system/weather-web.service`
Now add these lines in the text editor, and close by pressing `CTRL+X` and confirming with `Y`: 
```
[Unit]
Description=ASP.NET Core 3.1 Weather - Web

[Service]
WorkingDirectory=/htdocs/netcore/_tmp
ExecStart=/home/pi/dotnet-arm32/dotnet /htdocs/netcore/weather/net.jancerveny.weatherstation.Client.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-weather-web
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target

```
*Tip:* You can paste from your Windows clipboard by right-clicking in the Putty window.

and a second service, to collect data:
`sudo nano /etc/sysystemd/system/weather-worker.service`
```
[Unit]
Description=ASP.NET Core 3.1 Weather - Service Worker
After=postgresql.service

[Service]
WorkingDirectory=/etc/netcore-services/_tmp
ExecStart=/home/pi/dotnet-arm32/dotnet /etc/netcore-services/weather/net.jancerveny.weatherstation.WorkerService.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-weather-sw
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```
9. Run `sudo systemctl start weather-web` and `sudo systemctl start weather-worker` respectively.
10. You need to  set up the HTTP server (that's NginX, installed in Required prerequisities as a part of point 2.) to serve the outputs of the web service we just created above. Enter `sudo nano /etc/nginx/sites-available/default`, and replace all contents (if you aren't running anything else that you'd be aware of) with:
```
server {
    listen        80 default_server;
    listen        [::]:80 default_server;
    server_name   _;

	location ~* /(css|js|lib) {
        root          /htdocs/netcore/weather/wwwroot;
    }
    
	location / {
        proxy_pass         http://localhost:5050;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```
11. Try accessing **http://<Your Rasberry PI's IP address>**, and you should see a page [like my own](https://weather.jancerveny.net). If you made it this far, congratulations! If not, [get in touch with me](https://www.jancerveny.net).

### Making the temperatures accessible from anywhere, on the internet (WAN)
I will describe what needs to be done in broad terms, because the settings will be individual at this point.
1. Get a static IP address from your internet provider, if you can. I won't cover dynamic dns setup in this tutorial.
2. Set a static [local IP address to your Raspberry PI](https://thepihut.com/blogs/raspberry-pi-tutorials/how-to-give-your-raspberry-pi-a-static-ip-address-update), so the routing works even after Raspberry/router restart  or in case of power outtage.
3. Create a subdomain on your existing domain, and forward the **A record** to your new poublic static IP address you will have obtained from your internet provider.
4. Set up your home router to forward **port 80** (TCP and UDP) to your local Raspberry PI's IP address. Forward port 443 too, for SSL.
5. SSH to your Raspberry PI, and change the NginX settings to respond to your domain name. Replace `YOURDOMAIN` with your actual domain:
```
server {
    listen        80;
    listen        [::]:80;
    server_name   weather.YOURDOMAIN.com;
    
	# NOTE: Same settings as above
}
```
6. [Install certbot](https://certbot.eff.org/lets-encrypt/debianbuster-nginx), and run through its very convenient wizard that will set up automatic free SSL renewal to make your site secure.