using System;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Core
{
    public class DateTimeUtil
    {
        private readonly SettingRepository settings;

        public DateTimeUtil(SettingRepository settings)
        {
            this.settings = settings;
        }

        public DateTime ConvertUtcToMyTimeZone(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, settings.TimeZone);
        }

        public DateTime? ConvertUtcToMyTimeZone(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return ConvertUtcToMyTimeZone(dateTime.Value);
            return null;
        }

        public DateTime ConvertMyTimeZoneToUtc(DateTime dateTime)
        {
            if (settings.TimeZone == TimeZoneInfo.Utc)
            {
                return dateTime;
            }
            return TimeZoneInfo.ConvertTime(dateTime, settings.TimeZone, TimeZoneInfo.Utc);
        }

        public DateTime? ConvertMyTimeZoneToUtc(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return ConvertMyTimeZoneToUtc(dateTime.Value);
            return null;
        }
    }
}