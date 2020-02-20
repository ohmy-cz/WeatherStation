using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.jancerveny.weatherstation.DataLayer.Models
{
    public partial class Temperatures
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int SensorId { get; set; }
        public int Temperature { get; set; }
    }
}
