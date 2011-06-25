using System;

namespace FujiyBlog.Core.Common
{
    public class DateTimeUtil
    {
        public static DateTime ConvertUtcToMyTimeZone(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }

        public static DateTime ConvertMyTimeZoneToUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }
    }
}