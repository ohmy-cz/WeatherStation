using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.Common.Helpers;
using net.jancerveny.weatherstation.Common.Models;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
    /// <summary>
    /// Class that gathers methods to fetch new data from different sources
    /// </summary>
    public class DataCollectionService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly PhilipsHueConfiguration _config;
        private readonly WeatherProviderConfiguration _weatherConfig;
        private readonly ILogger<DataCollectionService> _logger;
		private readonly ILocalHueClient _hueClient;
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;
		public DataCollectionService(ILogger<DataCollectionService> logger, PhilipsHueConfiguration config, DbContextOptions<WeatherDbContext> dbOptions, IHttpClientFactory clientFactory, WeatherProviderConfiguration weatherConfiguration)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
            if (weatherConfiguration == null) throw new ArgumentNullException(nameof(weatherConfiguration));
            _logger = logger;
			_config = config;
			_dbOptions = dbOptions;
			_hueClient = new LocalHueClient(_config.BridgeIp);
			_hueClient.Initialize(_config.AppKey);
            _clientFactory = clientFactory;
            _weatherConfig = weatherConfiguration;
        }

		public async Task<bool> FetchTemperaturesAsync()
		{
			using (var db = new WeatherDbContext(_dbOptions))
			{
				try
				{
					var temperatures = new List<Measurement>();
                    temperatures.AddRange(await FetchWeatherForecastAsync());
                    var hueTemps = await FetchPhilipsHueSensorsAsync();

                    // Only update newer readings
					if (hueTemps != null)
					{
                        foreach(var hueTemp in hueTemps)
                        {
                            var lastMeasurement = db.Measurements.Where(y => hueTemp.SourceId == y.SourceId).OrderByDescending(y => y.Timestamp).Take(1).FirstOrDefault();
                            if(lastMeasurement == null || hueTemp.Timestamp > lastMeasurement.Timestamp)
                            {
                                temperatures.Add(hueTemp);
                            }
                        }
					}

					var piTemps = FetchRaspberryPi(); // Seems not working in Async mode
					if (piTemps != null)
					{
						temperatures.AddRange(piTemps);
					}

					_logger.LogInformation($"Amount of temperatures: {temperatures.Count()}");
					if (temperatures.Count > 0)
					{
						db.Measurements.AddRange(temperatures);
						return db.SaveChanges() > 0;
					}
				} 
				catch (Exception e)
				{
					_logger.LogError(e, $"Exception occured {e.Message}");
				}
				return false;
			}
        }

        /// <summary>
        /// Scan and update for new Philips Hue sensors.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ScanAndUpdate()
        {
            using (var db = new WeatherDbContext(_dbOptions))
            {
                var sensors = await _hueClient.GetSensorsAsync();
                var temperatureSensors = sensors.Where(x => x.Type == "ZLLTemperature");
                var presenceSensors = sensors.Where(x => x.Type == "ZLLPresence");
                var knownSensors = db.DataSources.ToList();
                var knownSensorIds = knownSensors.Select(x => x.Id);
                var newSensors = temperatureSensors.ToList().Where(x => !knownSensorIds.Contains(int.Parse(x.Id)));
                Colors.AssignedColors = knownSensors.Select(x => x.Color).ToList();

                // Add or update Philips Hue sensors
                foreach (var sensor in temperatureSensors)
                {
                    try
                    {
                        // The temperature sensors don't carry the user-assigned name - it's the presence ensors with the same unique id. We need to get rid of the dash and foru digits suffix.
                        var name = presenceSensors.Where(x => x.UniqueId.Substring(0, x.UniqueId.Length - 5) == sensor.UniqueId.Substring(0, x.UniqueId.Length - 5)).FirstOrDefault()?.Name ?? sensor.Name;
                        if (newSensors.Contains(sensor))
                        {
                            _logger.LogInformation($"Adding new Philips Hue sensor: {sensor.Id} - {name}");
                            db.DataSources.Add(new DataSource
                            {
                                Id = int.Parse(sensor.Id),
                                Created = sensor.State.Lastupdated ?? DateTime.Now,
                                Name = name,
                                LastRead = sensor.State.Lastupdated ?? DateTime.Now,
                                Color = Colors.TryGetUniqueColor(),
                                SourceType = SourceTypeEnum.PhilipsHue
                            });
                        }
                        else
                        {
                            var updateCandidate = knownSensors.Where(x => x.Id == int.Parse(sensor.Id) && x.Name != name).FirstOrDefault();
                            if (updateCandidate != null)
                            {
                                _logger.LogInformation($"Changing Philips Hue sensor name: {sensor.Id} - {updateCandidate.Name} to {name}");
                                updateCandidate.Name = name;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error occured while trying to update Philips Hue sensor Data Source.", e);
                    }
                }

                // Add Raspberry PI temperature sensors
                if (!knownSensorIds.Contains((int)ReservedSourceIdEnum.RaspberryPICPU))
                {
                    _logger.LogInformation("Adding new Raspberry CPU Temperature sensor");
                    db.DataSources.Add(new DataSource
                    {
                        Id = (int)ReservedSourceIdEnum.RaspberryPICPU,
                        Created = DateTime.Now,
                        Name = "Raspberry PI CPU",
                        LastRead = DateTime.Now,
                        Color = Colors.RaspberryPICPU,
                        SourceType = SourceTypeEnum.RaspberryPi
                    });
                }

                if (!knownSensorIds.Contains((int)ReservedSourceIdEnum.RaspberryPIGPU))
                {
                    _logger.LogInformation("Adding new Raspberry GPU Temperature sensor");
                    db.DataSources.Add(new DataSource
                    {
                        Id = (int)ReservedSourceIdEnum.RaspberryPIGPU,
                        Created = DateTime.Now,
                        Name = "Raspberry PI GPU",
                        LastRead = DateTime.Now,
                        Color = Colors.RaspberryPIGPU,
                        SourceType = SourceTypeEnum.RaspberryPi
                    });
                }

                // Add 3rdparty Weather forecast
                if (!knownSensorIds.Contains((int)ReservedSourceIdEnum.ThirdPartyWeather))
                {
                    _logger.LogInformation("Adding new Third party temperature source");
                    db.DataSources.Add(new DataSource
                    {
                        Id = (int)ReservedSourceIdEnum.ThirdPartyWeather,
                        Created = DateTime.Now,
                        Name = "Online",
                        LastRead = DateTime.Now,
                        Color = Colors.ThirdPartyWeather,
                        SourceType = SourceTypeEnum.ThirdPartyWeather
                    });
                }

                // Add 3rdparty Weather forecast - feels like temperature
                if (!knownSensorIds.Contains((int)ReservedSourceIdEnum.ThirdPartyWeatherFeelsLike))
                {
                    _logger.LogInformation("Adding new Third party temperature source");
                    db.DataSources.Add(new DataSource
                    {
                        Id = (int)ReservedSourceIdEnum.ThirdPartyWeatherFeelsLike,
                        Created = DateTime.Now,
                        Name = "Online - feels like",
                        LastRead = DateTime.Now,
                        Color = Colors.ThirdPartyWeatherFeelsLike,
                        SourceType = SourceTypeEnum.ThirdPartyWeather
                    });
                }

                return db.SaveChanges() > 0;
            }
        }

        private async Task<IReadOnlyCollection<Measurement>> FetchPhilipsHueSensorsAsync()
		{
			_logger.LogInformation("Fetching information from Philips Hue Sensors");
			var sensors =  await _hueClient.GetSensorsAsync();
			return sensors.Where(x => x.State.Temperature != null).Select(x => new Measurement { 
				SourceId = int.Parse(x.Id),
				Temperature = x.State.Temperature.Value,
				Timestamp = x.State.Lastupdated?.ToLocalTime() ?? DateTime.Now
			}).ToList();
		}

		private IReadOnlyCollection<Measurement> FetchRaspberryPi()
		{
			_logger.LogInformation("Fetching information from Raspberry PI");
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
			    {
                    //var a = await "cat /sys/class/thermal/thermal_zone0/temp".BashAsync();
                    //var b = await "/opt/vc/bin/vcgencmd measure_temp | grep -o -E '[0-9.]+'".BashAsync();
                    int.TryParse("cat /sys/class/thermal/thermal_zone0/temp".Bash(), out int cpuTemp);
                    int.TryParse("/opt/vc/bin/vcgencmd measure_temp | grep -o -E '[0-9.]+' | awk '{ RES = $1*100} END { print RES }'".Bash(), out int gpuTemp);

                    var now = DateTime.Now;
                    return new List<Measurement> {
                        new Measurement {
                            SourceId = (int)ReservedSourceIdEnum.RaspberryPICPU,
						    Temperature = cpuTemp / 10,
                            Timestamp = now
                        },
                        new Measurement {
                            SourceId = (int)ReservedSourceIdEnum.RaspberryPIGPU,
						    Temperature = gpuTemp,
                            Timestamp = now
                        }
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Could not fetch RPI info", e);
            }

            _logger.LogInformation("Not a Linux machine, skipping.");
			return null;
		}

        private async Task<IReadOnlyCollection<Measurement>> FetchWeatherForecastAsync()
        {
            _logger.LogInformation("Fetching information from Weather forecast provider");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_weatherConfig.Endpoint}?lat={_weatherConfig.Latitude}&lon={_weatherConfig.Longitude}&APPID={_weatherConfig.ApiKey}&units=metric");
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                var openWeatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(await response.Content.ReadAsStringAsync());
                var now = DateTime.Now;

                return new List<Measurement>
                {
                    new Measurement
                    {
                        SourceId = (int)ReservedSourceIdEnum.ThirdPartyWeather,
                        Temperature = (int)(openWeatherResponse.Main.Temp * 100),
                        Timestamp = now
                    },
                    new Measurement
                    {
                        SourceId = (int)ReservedSourceIdEnum.ThirdPartyWeatherFeelsLike,
                        Temperature = (int)(openWeatherResponse.Main.FeelsLike * 100),
                        Timestamp = now
                    }
                };
            } catch (Exception ex)
            {
                _logger.LogError("An error occured while trying to fetch external weather information", ex);
            }
            return null;
        }
    }
}
