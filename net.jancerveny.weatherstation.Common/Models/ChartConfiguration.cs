using net.jancerveny.weatherstation.Common.Interfaces;
using System;

namespace net.jancerveny.weatherstation.Common.Models
{
    public static class ChartConfiguration
    {
        public static TimeSpan Day = new TimeSpan(24, 0, 0);
        public static TimeSpan FiveMinutes = new TimeSpan(0, 5, 0);
        public static ChartJs RealTime => new ChartJs {
            Name = "Real time",
            ChartType = ChartTypeEnum.RealTime,
            Labels = new IChartJsLabels[] {
                new ChartJsLabels {
                    Label = $"{20 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-20 * 5)
                },
                new ChartJsLabels {
                    Label = $"{19 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-19 * 5)
                },
                new ChartJsLabels {
                    Label = $"{18 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-18 * 5)
                },
                new ChartJsLabels {
                    Label = $"{17 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-17 * 5)
                },
                new ChartJsLabels {
                    Label = $"{16 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-16 * 5)
                },
                new ChartJsLabels {
                    Label = $"{15 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-15 * 5)
                },
                new ChartJsLabels {
                    Label = $"{14 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-14 * 5)
                },
                new ChartJsLabels {
                    Label = $"{13 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-13 * 5)
                },
                new ChartJsLabels {
                    Label = $"{12 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-12 * 5)
                },
                new ChartJsLabels {
                    Label = $"{11 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-11 * 5)
                },
                new ChartJsLabels {
                    Label = $"{10 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-10 * 5)
                },
                new ChartJsLabels {
                    Label = $"{9 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-9 * 5)
                },
                new ChartJsLabels {
                    Label = $"{8 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-8 * 5)
                },
                new ChartJsLabels {
                    Label = $"{7 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-7 * 5)
                },
                new ChartJsLabels {
                    Label = $"{6 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-6 * 5)
                },
                new ChartJsLabels {
                    Label = $"{5 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-5 * 5)
                },
                new ChartJsLabels {
                    Label = $"{4 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-4 * 5)
                },
                new ChartJsLabels {
                    Label = $"{3 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-3 * 5)
                },
                new ChartJsLabels {
                    Label = $"{2 * 5}m ago",
                    Span = FiveMinutes,
                    Start = DateTime.Now.AddMinutes(-2 * 5)
                },
                new ChartJsLabels {
                    Label = "Now",
                    Span = new TimeSpan(0, 5, 0),
                    Start = DateTime.Now.AddMinutes(-1 * 5)
                }
            }
        };

        public static ChartJs Last7Days => new ChartJs
        {
            Name = "Daily",
            ChartType = ChartTypeEnum.Last7Days,
            Labels = new IChartJsLabels[] {
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-7).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-7)
                },
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-6).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-6)
                },
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-5).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-5)
                },
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-4).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-4)
                },
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-3).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-3)
                },
                new ChartJsLabels {
                    Label = DateTime.Today.AddDays(-2).ToShortDateString(),
                    Span = Day,
                    Start = DateTime.Today.AddDays(-2)
                },
                new ChartJsLabels {
                    Label = "Yesterday",
                    Span = Day,
                    Start = DateTime.Today.AddDays(-1)
                }
            }
        };
    }

    public class ChartJsLabels : IChartJsLabels {
        public string Label { get; set; }
        public TimeSpan Span { get; set; }
        public DateTime Start { get; set; }
    }

    public class ChartJs : IChartConfiguration
    {
        public string Name { get; set; }
        public ChartTypeEnum ChartType { get; set; }
        public IChartJsLabels[] Labels { get; set; }
    }
}
