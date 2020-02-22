using net.jancerveny.weatherstation.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.jancerveny.weatherstation.DataLayer.Models
{
    public partial class DataSource
    {
        /// <summary>
        /// The 3rd party ID (Philips hue sensor ID, or other vendor ID)
        /// </summary>
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ColorRGBA Color { get; set; }
        public SourceTypeEnum SourceType { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastRead { get; set; }
        public DataSource() 
        { 
        }

        public DataSource(int id, string name, byte r = 0, byte g = 0, byte b = 0, float a = 0.5F)
        {
            Id = id;
            Name = name;
            Color = new ColorRGBA(r, g, b, a);
        }
    }
}
