using System.Text.Json.Serialization;

namespace net.jancerveny.weatherstation.Common.Models
{
	public class OpenWeatherResponse
	{
		[JsonPropertyName("main")]
		public OpenWeatherResponseMain Main { get; set; }
	}

	public class OpenWeatherResponseMain
	{
		[JsonPropertyName("temp")]
		public double Temp { get; set; }
		[JsonPropertyName("feels_like")]
		public double FeelsLike { get; set; }
	}
}
