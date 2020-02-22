using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	/// <summary>
	/// Class that gathers methods to fetch new data from different sources
	/// </summary>
	public class DataCollection
	{
		private readonly ILogger<DataCollection> _logger;
		public DataCollection(ILogger<DataCollection> logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			_logger = logger;
		}
		public async Task<bool> FetchSensorsAsync()
		{
			_logger.LogInformation("Fetching information from Philips Hue Sensors");
			_logger.LogInformation("Fetching information from Raspberry PI");
			_logger.LogInformation("Fetching information from Weather forecast provider");
			return true;
		}
	}
}
