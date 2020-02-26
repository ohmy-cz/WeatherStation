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
		private readonly DataCollectionService _dc;
		private readonly ServiceConfiguration _sc;

		public DataCollectionWorker(ILogger<DataCollectionWorker> logger, DataCollectionService dc, ServiceConfiguration sc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (dc == null) throw new ArgumentNullException(nameof(dc));
			if (sc == null) throw new ArgumentNullException(nameof(sc));
			_logger = logger;
			_dc = dc;
			_sc = sc;
			_logger.LogInformation($"Scheduling Data Collection Worker to run every {TimeSpan.FromMilliseconds(_sc.FetchInterval):hh\\:mm\\:ss}");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Data Collection Worker running at: {time}", DateTimeOffset.Now);
				try
				{
					await _dc.ScanAndUpdate();
					await _dc.FetchSensorsAsync();
				} catch(Exception e)
				{
					_logger.LogError("Something failed while collecting data.", e);
				}
				await Task.Delay(_sc.FetchInterval, stoppingToken);
			}
		}
	}
}
