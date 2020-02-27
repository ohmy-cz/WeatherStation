using net.jancerveny.weatherstation.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.jancerveny.weatherstation.Common.Interfaces
{
    public interface IChartConfiguration
    {
        string Name { get; set; }
        ChartTypeEnum ChartType { get; set; }
        IChartJsLabels[] Labels { get; set; }
    }
}
