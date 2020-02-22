using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.Common.Models;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	/// <summary>
	/// Class that gathers methods to fetch new data from different sources
	/// </summary>
	public class DataCollectionService
	{
		private readonly PhilipsHueConfiguration _config;
		private readonly ILogger<DataCollectionService> _logger;
		private readonly ILocalHueClient _hueClient;
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;
		public DataCollectionService(ILogger<DataCollectionService> logger, PhilipsHueConfiguration config, DbContextOptions<WeatherDbContext> dbOptions)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
			_logger = logger;
			_config = config;
			_dbOptions = dbOptions;
			_hueClient = new LocalHueClient(_config.BridgeIp);
			_hueClient.Initialize(_config.AppKey);
		}
		public async Task<bool> FetchSensorsAsync()
		{
			using (var db = new WeatherDbContext(_dbOptions))
			{
				try
				{
					var temperatures = new List<Measurement>();
					var hueTemps = await FetchPhilipsHueSensorsAsync();
					if (hueTemps != null)
					{
						temperatures.AddRange(hueTemps);
					}
					var piTemps = await FetchRaspberryPiAsync();
					if (piTemps != null)
					{
						temperatures.AddRange(piTemps);
					}
					//temperatures.Add(await FetchWeatherForecastAsync());
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

		private async Task<IReadOnlyCollection<Measurement>> FetchPhilipsHueSensorsAsync()
		{
			_logger.LogInformation("Fetching information from Philips Hue Sensors");
			var sensors =  await _hueClient.GetSensorsAsync();
			return sensors.Where(x => x.State.Temperature != null).Select(x => new Measurement { 
				SourceId = int.Parse(x.Id),
				Temperature = x.State.Temperature.Value,
				Timestamp = DateTime.Now
			}).ToList();
		}

		private async Task<IReadOnlyCollection<Measurement>> FetchRaspberryPiAsync()
		{
			_logger.LogInformation("Fetching information from Raspberry PI");
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				int.TryParse(await "cat /sys/class/thermal/thermal_zone0/temp".BashAsync(), out int cpuTemp);
				double.TryParse(await "/opt/vc/bin/vcgencmd measure_temp | grep -o -E '[0-9.]+'".BashAsync(), out double gpuTemp);

				var now = DateTime.Now;
				return new List<Measurement> {
					new Measurement {
						SourceId = -2, // = CPU
						Temperature = cpuTemp / 10,
						Timestamp = now
					},
					new Measurement {
						SourceId = -3, // = GPU
						Temperature = (int)(gpuTemp * 100),
						Timestamp = now
					}
				};
			}

			_logger.LogInformation("Not a Linux machine, skipping.");
			return null;
		}

		//private async Task<Temperatures> FetchWeatherForecastAsync()
		//{
		// _logger.LogInformation("Fetching information from Weather forecast provider");
		//	return new NotImplementedException();
		//}
	}
}
