using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.BusinessLayer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly DataCollection _dc;

		public Worker(ILogger<Worker> logger, DataCollection dc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (dc == null) throw new ArgumentNullException(nameof(dc));
			_logger = logger;
			_dc = dc;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				await _dc.FetchSensorsAsync();
				// TODO: Aggregation service
				var intervalSeconds = 5 * 60 * 1000;
#if DEBUG
				intervalSeconds = 60 * 1000;
#endif
				await Task.Delay(intervalSeconds, stoppingToken);
			}
		}
	}
}
