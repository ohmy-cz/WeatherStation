using net.jancerveny.weatherstation.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace net.jancerveny.weatherstation.Common.Helpers
{
    public static class Colors
    {
        public static ColorRGBA[] ColorsList => new ColorRGBA[] {
            new ColorRGBA(179, 174, 151, 0.2F),
            new ColorRGBA(89, 58, 84, 0.2F),
            new ColorRGBA(165, 124, 158, 0.2F),
            new ColorRGBA(73, 114, 112, 0.2F),
            new ColorRGBA(168, 242, 237, 0.2F)
        };
        public static IList<ColorRGBA> AssignedColors = new List<ColorRGBA>();

        public static ColorRGBA RaspberryPICPU => new ColorRGBA(197, 26, 74, 0.2F);
        public static ColorRGBA RaspberryPIGPU => new ColorRGBA(100, 0, 30, 0.2F);
        public static ColorRGBA ThirdPartyWeather => new ColorRGBA(66, 66, 66, 0.2F);
        public static ColorRGBA TryGetUniqueColor() {
            // Select a new color by using ToString(), so we don't compare objects, but rather their values
            var newColor = ColorsList.Where(x => !AssignedColors.Select(z => z.ToString()).Contains(x.ToString())).FirstOrDefault();
            if(newColor != null)
            {
                AssignedColors.Add(newColor); // Make sure the same color doesn't get used twice, if this helper function gets called multiple times in a loop
                return newColor;
            }
            return ColorsList[0];
        }
    }
}
