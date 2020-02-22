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
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
    public class DataSourcesService
    {
        private readonly PhilipsHueConfiguration _config;
        private readonly ILogger<DataCollectionService> _logger;
        private readonly ILocalHueClient _hueClient;
        private readonly DbContextOptions<WeatherDbContext> _dbOptions;
        public DataSourcesService(ILogger<DataCollectionService> logger, PhilipsHueConfiguration config, DbContextOptions<WeatherDbContext> dbOptions)
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
        /// <summary>
        /// Get a  list of all sources of data
        /// </summary>
        /// <returns>A list of tuples with id of source first, and its name second</returns>
        public IReadOnlyCollection<DataSource> GetAll()
        {
            using (var db = new WeatherDbContext(_dbOptions))
            {
                return db.DataSources.ToList();
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
                    try { 
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
                                //db.Entry(updateCandidate).State = EntityState.Modified; // TODO: Maybe not needed
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
                        Name = "Forcast",
                        LastRead = DateTime.Now,
                        Color = Colors.ThirdPartyWeather,
                        SourceType = SourceTypeEnum.WeatherForcast
                    });
                }

                return db.SaveChanges() > 0;
            }
        }
    }
}
