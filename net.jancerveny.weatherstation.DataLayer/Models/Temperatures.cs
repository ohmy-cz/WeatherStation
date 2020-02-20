using System;
using System.Collections.Generic;

namespace net.jancerveny.weatherstation.DataLayer.Models
{
    public partial class Temperatures
    {
        public DateTime Timestamp { get; set; }
        public int SensorId { get; set; }
        public long Id { get; set; }
        public int Temperature { get; set; }
    }
}
