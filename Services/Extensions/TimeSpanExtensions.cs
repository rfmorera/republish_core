using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToTimeString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh:mm tt");
        }
    }
}
