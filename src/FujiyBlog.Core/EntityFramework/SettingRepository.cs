using System;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Core.EntityFramework
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public SettingRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public int  MinRequiredPasswordLength
        {
            get { return int.Parse(Database.Settings.Find((int)SettingNames.MinRequiredPasswordLength).Value); }
            set { SaveSettings(SettingNames.MinRequiredPasswordLength, value.ToString()); }
        }

        public int PostsPerPage
        {
            get { return int.Parse(Database.Settings.Find((int)SettingNames.PostsPerPage).Value); }
            set { SaveSettings(SettingNames.PostsPerPage, value.ToString()); }
        }

        public string BlogName
        {
            get { return Database.Settings.Find((int)SettingNames.BlogName).Value; }
            set { SaveSettings(SettingNames.BlogName, value); }
        }

        public string BlogDescription
        {
            get { return Database.Settings.Find((int)SettingNames.BlogDescription).Value; }
            set { SaveSettings(SettingNames.BlogDescription, value); }
        }

        public string Theme
        {
            get { return Database.Settings.Find((int)SettingNames.Theme).Value; }
            set { SaveSettings(SettingNames.Theme, value); }
        }

        public TimeZoneInfo TimeZoneId
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(Database.Settings.Find((int)SettingNames.TimeZoneId).Value); }
            set { SaveSettings(SettingNames.TimeZoneId, value.Id); }
        }

        private void SaveSettings(SettingNames settings, string value)
        {
            Database.Settings.Find((int) settings).Value = value;
            Database.SaveChanges();
        }

        private enum SettingNames
        {
            MinRequiredPasswordLength = 1,
            PostsPerPage = 2,
            BlogName = 3,
            BlogDescription = 4,
            Theme = 5,
            TimeZoneId = 6,
        }
    }
}
