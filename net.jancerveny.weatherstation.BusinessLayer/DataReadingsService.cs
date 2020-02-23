using Microsoft.EntityFrameworkCore;
using net.jancerveny.weatherstation.BusinessLayer.Models;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	public class DataReadingsService
	{
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;
		public DataReadingsService(DbContextOptions<WeatherDbContext> dbOptions)
		{
			if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
			_dbOptions = dbOptions;
		}

		public async Task<ReadingsResponse> GetReadingsAsync(int limit, DateTime? since = null)
		{
			if(since == null)
			{
				since = DateTime.Now.AddHours(-24);
			}
			using (var db =  new WeatherDbContext(_dbOptions))
			{
				return new ReadingsResponse { 
					Readings = await db.Measurements.Where(x => x.Timestamp > since).Take(limit).ToListAsync(),
					Timestamp = DateTime.Now
				};
			}
		}
    }
}
