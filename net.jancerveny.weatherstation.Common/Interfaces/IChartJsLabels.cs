using System;

namespace net.jancerveny.weatherstation.Common.Interfaces
{
    public interface IChartJsLabels
    {
        string Label { get; set; }
        TimeSpan Span { get; set; }
        DateTime Start { get; set; }
    }
}
