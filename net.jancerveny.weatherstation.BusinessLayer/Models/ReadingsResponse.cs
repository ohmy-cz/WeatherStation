using net.jancerveny.weatherstation.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace net.jancerveny.weatherstation.BusinessLayer.Models
{
    public class ReadingsResponse
    {
        public IList<IReadOut> Readings { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
