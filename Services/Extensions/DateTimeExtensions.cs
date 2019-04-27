using System;
using System.Globalization;

namespace Republish.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateTimeString(this DateTime? dateTime)
        {
            return ToDateStringCommon(dateTime, ToDateTimeString);
        }

        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(@"MM/dd/yyyy hh:mm:ss tt");
        }

        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(@"hh:mm tt");
        }

        public static string ToDateString(this DateTime? dateTime)
        {
            return ToDateStringCommon(dateTime, ToDateString);
        }

        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString(@"MM\/dd\/yyyy");
        }

        public static DateTime? ToNullableDateTime(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.ParseExact(value, "MM/dd/yyyy", new CultureInfo("en-US"));
            }
            else
            {
                return null;
            }
        }

        public static DateTime ToUtcCuba(this DateTime dateTime)
        {
            TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            DateTime utc = TimeZoneInfo.ConvertTime(dateTime, hwZone);
            return utc;
        }
        public static DateTime ToDateTime(this string value)
        {
            return DateTime.ParseExact(value, "MM/dd/yyyy", new CultureInfo("en-US"));
        }

        private static string ToDateStringCommon(DateTime? dateTime, Func<DateTime, string> dateFunc)
        {
            if (dateTime.HasValue)
            {
                return dateFunc(dateTime.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
