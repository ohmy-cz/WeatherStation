using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.WorkerService.Models;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class DataAggregationWorker : IHostedService, IDisposable
	{
		private bool disposed;
		private readonly ILogger<DataAggregationWorker> _logger;
		private readonly DataAggregationService _da;
		private readonly ServiceConfiguration _sc;
		private Timer _timer;
		private readonly TimeSpan _aggregationInterval;

		public DataAggregationWorker(ILogger<DataAggregationWorker> logger, DataAggregationService da, ServiceConfiguration sc)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (da == null) throw new ArgumentNullException(nameof(da));
			if (sc == null) throw new ArgumentNullException(nameof(sc));
			_logger = logger;
			_da = da;
			_sc = sc;
			_aggregationInterval = TimeSpan.FromMilliseconds(_sc.AggregateInterval);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var dueTime = Common.Helpers.Time.TodayMidnight();
#if DEBUG
			dueTime = TimeSpan.Zero;
#endif
			_logger.LogInformation($"Scheduling Data Aggregation Worker to run in: {dueTime:dd\\ \\d\\a\\y\\s\\,\\ hh\\:mm\\:ss} in interval {_aggregationInterval:dd\\ \\d\\a\\y\\s\\,\\ hh\\:mm\\:ss}");
			_timer = new Timer(DoWork, null, dueTime, _aggregationInterval);
			return Task.CompletedTask;
		}
		private void DoWork(object state)
		{
			_logger.LogInformation($"Data Aggregation Worker running at: {DateTimeOffset.Now}");
			try
			{
				_ = _da.AggregateAsync(1);
			}
			catch (NpgsqlException e)
			{
				_logger.LogError("Aggregation failed due to SQL issue.", e);
			}
			catch (Exception e)
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
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				_timer?.Dispose();
			}

			disposed = true;
		}

		~DataAggregationWorker()
		{
			Dispose(false);
		}
	}
}
