using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.DataLayer;
using System;
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
			return true;
		}
	}
}
