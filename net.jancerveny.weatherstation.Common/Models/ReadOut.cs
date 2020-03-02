using net.jancerveny.weatherstation.Common.Interfaces;

namespace net.jancerveny.weatherstation.Common.Models
{
    public class ReadOut : IReadOut
    {
        public int SourceId { get; set; }
        public int? Temperature { get; set; }
        /// <summary>
        /// This readout is the last known value, because no newer readout was available than the last fetched.
        /// </summary>
        public bool Stale { get; set; }
    }
}
