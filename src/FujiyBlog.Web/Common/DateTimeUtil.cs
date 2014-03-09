using System;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Common
{
    public class DateTimeUtil
    {
        public static DateTime ConvertUtcToMyTimeZone(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, Settings.SettingRepository.TimeZone);
        }

        public static DateTime? ConvertUtcToMyTimeZone(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return ConvertUtcToMyTimeZone(dateTime.Value);
            return null;
        }

        public static DateTime ConvertMyTimeZoneToUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, Settings.SettingRepository.TimeZone);
        }

        public static DateTime? ConvertMyTimeZoneToUtc(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return ConvertMyTimeZoneToUtc(dateTime.Value);
            return null;
        }
    }
}