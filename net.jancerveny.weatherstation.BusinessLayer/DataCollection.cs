using System;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.BusinessLayer
{
	/// <summary>
	/// Class that gathers methods to fetch new data from different sources
	/// </summary>
	public class DataCollection
	{
		public async Task<bool> FetchSensorsAsync()
		{
			return true;
		}
	}
}
