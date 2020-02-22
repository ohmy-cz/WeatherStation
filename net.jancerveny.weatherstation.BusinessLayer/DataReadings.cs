using Microsoft.EntityFrameworkCore;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	public class DataReadings
	{
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;
		public DataReadings(DbContextOptions<WeatherDbContext> dbOptions)
		{
			if (dbOptions == null) throw new ArgumentNullException(nameof(dbOptions));
			_dbOptions = dbOptions;
		}

		public IReadOnlyCollection<Temperatures> GetReadings()
		{
			using (var db =  new WeatherDbContext(_dbOptions))
			{
				return db.Temperatures.ToList();
			}
		}
    }
}
