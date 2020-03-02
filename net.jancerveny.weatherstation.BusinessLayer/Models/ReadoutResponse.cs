using net.jancerveny.weatherstation.Common.Interfaces;
using net.jancerveny.weatherstation.Common.Models;
using System;
using System.Collections.Generic;

namespace net.jancerveny.weatherstation.BusinessLayer.Models
{
    public class ReadoutResponse
    {
        public IList<IReadOut> Readouts { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
