namespace net.jancerveny.weatherstation.WorkerService.Models
{
    public class ServiceConfiguration
    {
        /// <summary>
        /// Interval in which to fetch new sensor data in Seconds
        /// </summary>
        public int FetchInterval { get; set; }
        /// <summary>
        /// Interval in which to aggregate data
        /// </summary>
        public int AggregateInterval { get; set; }
    }
}
