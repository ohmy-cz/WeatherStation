using Microsoft.EntityFrameworkCore;
using net.jancerveny.weatherstation.BusinessLayer.Models;
using net.jancerveny.weatherstation.Common.Interfaces;
using net.jancerveny.weatherstation.Common.Models;
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

		public async Task<ReadingsResponse> GetReadingsAsync(DateTime? customSince = null)
		{
			var since = ChartConfiguration.RealTime.Labels.First().Start;
			if (customSince != null)
			{
				since = customSince.Value;
			}
			using (var db =  new WeatherDbContext(_dbOptions))
			{
				var sources = await db.DataSources.ToListAsync();
				var measurements = await db.Measurements.Where(x => x.Timestamp > since).ToListAsync();
				var readouts = new List<IReadOut>();
				var labels = ChartConfiguration.RealTime.Labels.ToList();
				if(customSince != null)
				{
					//  we will only fetch the data for the last "frame", other data already exists in the frontend
					labels = labels.Where(x => customSince.Value > x.Start).TakeLast(1).ToList();
				}

				// TODO remove
				var lastNonNullTemperatures = new Dictionary<int, int?>();
				foreach (var source in sources)
				{
					lastNonNullTemperatures.Add(source.Id, null);
				}

				foreach (var chartLabel in labels)
				{
					var medianTemperatures = new Dictionary<int,IEnumerable<double>>();
					foreach (var source in sources)
					{
						var medianTemperature = measurements
							.Where(x =>
								x.SourceId == source.Id &&
								x.Timestamp >= chartLabel.Start &&
								x.Timestamp < chartLabel.Start.AddMilliseconds(chartLabel.Span.TotalMilliseconds)
							)
							.Select(x => (double)x.Temperature);

						medianTemperatures.Add(source.Id, medianTemperature);
					}

					if(medianTemperatures.Where(x => x.Value.Count() == 0).Count() == sources.Count())
					{
						// If all measureents for all sensors were empty, skip this frame.
						continue;
					}

					foreach(var medianTemperature in medianTemperatures)
					{
						if (medianTemperature.Value != null && medianTemperature.Value.Count() > 0)
						{
							lastNonNullTemperatures[medianTemperature.Key] = (int?)Math.Round(medianTemperature.Value.Median() * 100) / 100;
						}
					}

					foreach (var source in sources)
					{
						readouts.Add(new ReadOut
						{
							SourceId = source.Id,
							Temperature = lastNonNullTemperatures[source.Id]
						});
					}
				}

				// Hack, the last set of readings always seems to come up empty
				//var readouts1 = readouts.Take(readouts.Count() - sources.Count()).ToList();

				return new ReadingsResponse { 
					Readings = readouts,
					Timestamp = DateTime.Now
				};
			}
		}

		public async Task<IReadOnlyCollection<AggregatedMeasurement>> GetAggregationsAsync(DateTime start, DateTime? end = null)
		{	
			using (var db = new WeatherDbContext(_dbOptions))
			{
				return await db.AggregatedMeasurements
					.Where(x =>
						x.Day.Date >= start.Date &&
						(end != null ? x.Day.Date < end.Value.Date : true)
					)
					.ToListAsync();
			}
		}

	}
}
