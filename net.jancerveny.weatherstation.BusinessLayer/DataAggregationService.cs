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

		public async Task<bool> AggregateAsync()
		{
			_logger.LogInformation("Agregate");
			using (var db = new WeatherDbContext(_dbOptions))
			{
				var measurements = db.Measurements.Where(x => x.Timestamp <= DateTime.Now.AddDays(-7));
				if(measurements.Count() == 0)
				{
					return true;
				}
				//or Lambda https://stackoverflow.com/questions/15696817/translate-sql-to-lambda-linq-with-groupby-and-average
				var aggregate = measurements.GroupBy(x => new { x.SourceId }).Select(g => new AggregatedMeasurement { SourceId = g.Key.SourceId, Temperature = (int)Math.Round(g.Average(p => p.Temperature))  }).ToList();
				db.AggregatedMeasurements.AddRange(aggregate);
				db.Measurements.RemoveRange(measurements);
				return await db.SaveChangesAsync() > 0;
			}
		}

		public async Task<bool> Trim()
		{
			_logger.LogInformation("Trimming old measurements");
			using (var db = new WeatherDbContext(_dbOptions))
			{
				var measurements = db.Measurements.Where(x => x.Timestamp <= DateTime.Now.AddDays(-3));
				db.Measurements.RemoveRange(measurements);
				return await db.SaveChangesAsync() > 0;
			}
		}
	}
}
