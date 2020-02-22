using net.jancerveny.weatherstation.DataLayer.Models;
using System;

namespace net.jancerveny.weatherstation.DataLayer.Interfaces
{
    interface IMeasurement
    {
        long Id { get; set; }
        DateTime Timestamp { get; set; }
        int SourceId { get; set; }
        DataSource Source { get; set; }
        int Temperature { get; set; }
    }
}
