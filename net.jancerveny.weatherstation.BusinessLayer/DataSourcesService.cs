using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
    public class DataSourcesService
    {
        private readonly ILogger<DataSourcesService> _logger;
        private readonly DbContextOptions<WeatherDbContext> _dbOptions;
        public DataSourcesService(ILogger<DataSourcesService> logger, DbContextOptions<WeatherDbContext> dbOptions)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
            _logger = logger;
            _dbOptions = dbOptions;
        }
        /// <summary>
        /// Get a  list of all sources of data
        /// </summary>
        /// <returns>A list of tuples with id of source first, and its name second</returns>
        public async Task<IReadOnlyCollection<DataSource>> GetAllAsync()
        {
            using (var db = new WeatherDbContext(_dbOptions))
            {
                return await db.DataSources.ToListAsync();
            }
        }
    }
}
