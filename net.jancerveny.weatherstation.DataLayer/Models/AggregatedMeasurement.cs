using net.jancerveny.weatherstation.DataLayer.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.jancerveny.weatherstation.DataLayer.Models
{
    public partial class AggregatedMeasurement : IMeasurement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int SourceId { get; set; }
        public virtual DataSource Source { get; set; }
        public int Temperature { get; set; }
        /// <summary>
        /// Aggregation span = Timestamp + AggregationLength
        /// </summary>
        public AggregationLengthEnum AggregationLength { get; set; }
    }
}
