using System;

namespace net.jancerveny.weatherstation.Common.Models
{
	public class WeatherProviderConfiguration
	{
		public Uri Endpoint { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string ApiKey { get; set; }
	}
}
