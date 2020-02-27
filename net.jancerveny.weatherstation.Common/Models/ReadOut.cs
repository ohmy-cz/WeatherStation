using net.jancerveny.weatherstation.Common.Interfaces;

namespace net.jancerveny.weatherstation.Common.Models
{
    public class ReadOut : IReadOut
    {
        public int SourceId { get; set; }
        public int? Temperature { get; set; }
    }
}
