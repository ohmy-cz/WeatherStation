﻿using Microsoft.EntityFrameworkCore;
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
		/// <param name="maxAgeDays">Number of -D days to start aggregating from</param>
		/// <returns>True on success</returns>
		public async Task<bool> AggregateAsync(int maxAgeDays)
		{
			_logger.LogInformation($"Agregating old measurements with {maxAgeDays} days offset");
			using (var db = new WeatherDbContext(_dbOptions))
			{
				var oldestDay = db.Measurements.OrderBy(x => x.Timestamp)?.Take(1)?.Select(x => x.Timestamp)?.FirstOrDefault().Date;
				if(oldestDay == null)
				{
					_logger.LogInformation($"Nothing to aggregate.");
					return true;
				}

				var newestDay = DateTime.Today;
				if (maxAgeDays > 1)
				{
					newestDay.AddDays((maxAgeDays - 1) * -1);
				}
				int daysDelta = ((TimeSpan)(newestDay - oldestDay)).Days;
				if (daysDelta < 1)
				{
					_logger.LogInformation($"Nothing to aggregate, the oldest measured day {oldestDay:d} is after offset {newestDay:d}");
					return true;
				}

				for (var dayOffset = 0; dayOffset < daysDelta; dayOffset++)
				{
					var startDay = oldestDay.Value.AddDays(dayOffset); 
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
						).Select(x => 
							(double)x.Temperature
						).Median());

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
