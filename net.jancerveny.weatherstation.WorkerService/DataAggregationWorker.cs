using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.WorkerService.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class DataAggregationWorker : IHostedService, IDisposable
	{
		private readonly ILogger<DataAggregationWorker> _logger;
		private readonly DataAggregationService _da;
		private readonly ServiceConfiguration _sc;
		private Timer _timer;

		public DataAggregationWorker(ILogger<DataAggregationWorker> logger, DataAggregationService da, ServiceConfiguration sc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (da == null) throw new ArgumentNullException(nameof(da));
			if (sc == null) throw new ArgumentNullException(nameof(sc));
			_logger = logger;
			_da = da;
			_sc = sc;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var dueTime = Common.Helpers.Time.TodayMidnight();
			var interval = TimeSpan.FromMilliseconds(_sc.AggregateInterval);
			_logger.LogInformation($"Scheduling Data Aggregation Worker to run in: {dueTime:dd\\ \\d\\a\\y\\s\\,\\ hh\\:mm\\:ss} in interval {interval:dd\\ \\d\\a\\y\\s\\,\\ hh\\:mm\\:ss}");
			_timer = new Timer(DoWork, null, dueTime, interval);
			return Task.CompletedTask;
		}
		private void DoWork(object state)
		{
			_logger.LogInformation($"Data Aggregation Worker running at: {DateTimeOffset.Now}");
			try
			{
				_ = _da.Trim();
			} catch(Exception e)
			{
				_logger.LogError("Could not trim old measurements.", e);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
