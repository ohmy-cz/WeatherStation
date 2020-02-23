using net.jancerveny.weatherstation.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace net.jancerveny.weatherstation.BusinessLayer.Models
{
    public class ReadingsResponse
    {
        public IReadOnlyCollection<Measurement> Readings { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
