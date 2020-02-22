using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.WorkerService.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class DataAggregationWorker : BackgroundService
	{
		private readonly ILogger<DataCollectionWorker> _logger;
		private readonly DataAggregationService _da;
		private readonly ServiceConfiguration _sc;

		public DataAggregationWorker(ILogger<DataCollectionWorker> logger, DataAggregationService da, ServiceConfiguration sc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (da == null) throw new ArgumentNullException(nameof(da));
			if (sc == null) throw new ArgumentNullException(nameof(sc));
			_logger = logger;
			_da = da;
			_sc = sc;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Data Aggregation Worker running at: {time}", DateTimeOffset.Now);
				await _da.AggregateAsync();
				var intervalSeconds = _sc.AggregateInterval;
#if DEBUG
				intervalSeconds = 5 * 60 * 1000;
#endif
				await Task.Delay(intervalSeconds, stoppingToken);
			}
		}
	}
}
