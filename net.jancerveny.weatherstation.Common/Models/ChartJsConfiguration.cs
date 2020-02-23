using System;

namespace net.jancerveny.weatherstation.Common.Models
{
    public static class RealtimeChartConfiguration
    {
        public static AggregationLengthEnum Aggregation => AggregationLengthEnum.None;
        public static ChartJsLabels[] Labels => new ChartJsLabels[] { 
            new ChartJsLabels { 
                Label = "6 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-6)
            },
            new ChartJsLabels {
                Label = "5,5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-5).AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-5)
            },
            new ChartJsLabels {
                Label = "4,5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-4).AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "4 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-4)
            },
            new ChartJsLabels {
                Label = "3,5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-3).AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "3 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-3)
            },
            new ChartJsLabels {
                Label = "2,5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-2).AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "2 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-2)
            },
            new ChartJsLabels {
                Label = "1,5 hours ago",
                Span = new TimeSpan(0, 30, 0),
                Start = DateTime.Now.AddHours(-1).AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "1 hours ago",
                Span = new TimeSpan(0, 15, 0),
                Start = DateTime.Now.AddHours(-1)
            },
            new ChartJsLabels {
                Label = "45 mins ago",
                Span = new TimeSpan(0, 15, 0),
                Start = DateTime.Now.AddMinutes(-45)
            },
            new ChartJsLabels {
                Label = "30 mins ago",
                Span = new TimeSpan(0, 15, 0),
                Start = DateTime.Now.AddMinutes(-30)
            },
            new ChartJsLabels {
                Label = "15 mins ago",
                Span = new TimeSpan(0, 15, 0),
                Start = DateTime.Now.AddMinutes(-15)
            },
            new ChartJsLabels {
                Label = "5 mins ago",
                Span = new TimeSpan(0, 5, 0),
                Start = DateTime.Now.AddMinutes(-5)
            },
            new ChartJsLabels {
                Label = "Now",
                Span = new TimeSpan(0, 5, 0)
            },
        };
    }

    public class ChartJsLabels
    {
        public string Label { get; set; }
        public TimeSpan Span { get; set; }
        public DateTime? Start { get; set; }
    }
}
