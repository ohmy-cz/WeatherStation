using net.jancerveny.weatherstation.DataLayer.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.jancerveny.weatherstation.DataLayer.Models
{
    public partial class AggregatedMeasurement : IMeasurement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// Day of a year
        /// </summary>
        [Column(TypeName="Date")]
        public DateTime Day { get; set; }
        public int SourceId { get; set; }
        public virtual DataSource Source { get; set; }
        /// <summary>
        /// Average temperature for the day
        /// </summary>
        public int Temperature { get; set; }
    }
}
