using System;

namespace net.jancerveny.weatherstation.Common.Helpers
{
    public static class Time
    {
        public static TimeSpan TodayMidnight() {
            var midnightTonight = DateTime.Today.AddDays(1);
            var differenceInMilliseconds = (midnightTonight - DateTime.Now).TotalMilliseconds;
            return TimeSpan.FromMilliseconds(differenceInMilliseconds); 
        }
    }
}
