using System;
using System.Collections.Generic;
using System.Text;

namespace net.jancerveny.weatherstation.BusinessLayer
{
    public class DataSources
    {
        /// <summary>
        /// Get a  list of all sources of data
        /// </summary>
        /// <returns>A list of tuples with id of source first, and its name second</returns>
        public IReadOnlyCollection<Tuple<int, string>> GetAll()
        {
            var result = new List<Tuple<int, string>>();
            result.Add(new Tuple<int, string>(-2, "RPI CPU"));
            result.Add(new Tuple<int, string>(-3, "RPI GPU"));
            // TODO: make this dynamic
            result.Add(new Tuple<int, string>(-4, "Weather web"));
            result.Add(new Tuple<int, string>(15, "Living Room"));
            result.Add(new Tuple<int, string>(24, "Balcony"));
            result.Add(new Tuple<int, string>(47, "Kitchen"));
            return result;
        }
    }
}
