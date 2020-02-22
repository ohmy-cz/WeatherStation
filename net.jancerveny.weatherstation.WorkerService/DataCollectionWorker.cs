using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.WorkerService.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class DataCollectionWorker : BackgroundService
	{
		private readonly ILogger<DataCollectionWorker> _logger;
		private readonly DataSourcesService _dsrc;
		private readonly DataCollectionService _dc;
		private readonly ServiceConfiguration _sc;

		public DataCollectionWorker(ILogger<DataCollectionWorker> logger, DataCollectionService dc, ServiceConfiguration sc, DataSourcesService dsrc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (dc == null) throw new ArgumentNullException(nameof(dc));
			if (sc == null) throw new ArgumentNullException(nameof(sc));
			if (dsrc == null) throw new ArgumentNullException(nameof(dsrc));
			_logger = logger;
			_dc = dc;
			_sc = sc;
			_dsrc = dsrc;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Data Collection Worker running at: {time}", DateTimeOffset.Now);
				await _dsrc.ScanAndUpdate();
				await _dc.FetchSensorsAsync();
				var intervalSeconds = _sc.FetchInterval;
#if DEBUG
				intervalSeconds = 60 * 1000;
#endif
				await Task.Delay(intervalSeconds, stoppingToken);
			}
		}
	}
}
