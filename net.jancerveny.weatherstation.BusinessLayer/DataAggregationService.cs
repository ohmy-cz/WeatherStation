using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	public class DataAggregationService
	{
		private readonly ILogger<DataCollectionService> _logger;
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;
		public DataAggregationService(ILogger<DataCollectionService> logger, DbContextOptions<WeatherDbContext> dbOptions)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
			_logger = logger;
			_dbOptions = dbOptions;
		}

		/// <summary>
		/// Take measurements for last D days for each data source, average them for 24h period, store them in an aggregation table and delete them from the intermmediate measurements table.
		/// </summary>
		/// <param name="days">Number of complete days to aggregate</param>
		/// <param name="offsetDays">Number of -D days to start aggregating from</param>
		/// <returns>True on success</returns>
		public async Task<bool> AggregateAsync(int days, int offsetDays)
		{
			_logger.LogInformation("Agregating old measurements");
			using (var db = new WeatherDbContext(_dbOptions))
			{
				for (var day = 1; day <= days; day++)
				{
					var startDay = DateTime.Today.AddDays((day + offsetDays) * -1);
					var endDay = startDay.AddDays(1);
					
					// If an aggregation exists already (maybe this method ran before, or there was an overlap in timing),
					// skip aggregation
					if(db.AggregatedMeasurements.Where(x => x.Day.Date == startDay.Date).Any())
					{
						continue;
					}

					var dailyMeasurements = db.Measurements.Where(x =>
						x.Timestamp >= startDay &&
						x.Timestamp < endDay
					);

					var dataSources = dailyMeasurements
						.Select(x => x.SourceId)
						.Distinct()
						.ToList();

					foreach (var dataSourceId in dataSources)
					{
						var averageDailyTemperature = (int)Math.Round(dailyMeasurements.Where(x => 
							x.SourceId == dataSourceId
						).Average(x => 
							x.Temperature
						));

						db.AggregatedMeasurements.Add(new AggregatedMeasurement { 
							SourceId = dataSourceId,
							Temperature = averageDailyTemperature,
							Day = startDay
						});
					}
					
					// Remove the measurements from the live measurements table
					db.Measurements.RemoveRange(dailyMeasurements);
				}

				return await db.SaveChangesAsync() > 0;
			}
		}
	}
}
