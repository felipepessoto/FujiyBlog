using System;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Core.EntityFramework
{
    public class SettingRepository : RepositoryBase<Setting>
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

        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(Database.Settings.Find((int)SettingNames.TimeZone).Value); }
            set { SaveSettings(SettingNames.TimeZone, value.Id); }
        }

        public string EmailFrom
        {
            get { return Database.Settings.Find((int)SettingNames.EmailFrom).Value; }
            set { SaveSettings(SettingNames.EmailFrom, value); }
        }

        public string EmailSmtpAddress
        {
            get { return Database.Settings.Find((int)SettingNames.EmailSmtpAddress).Value; }
            set { SaveSettings(SettingNames.EmailSmtpAddress, value); }
        }

        public int EmailSmtpPort
        {
            get { return int.Parse(Database.Settings.Find((int)SettingNames.EmailSmtpPort).Value); }
            set { SaveSettings(SettingNames.EmailSmtpPort, value.ToString()); }
        }

        public string EmailUserName
        {
            get { return Database.Settings.Find((int)SettingNames.EmailUserName).Value; }
            set { SaveSettings(SettingNames.EmailUserName, value); }
        }

        public string EmailPassword
        {
            get { return Database.Settings.Find((int)SettingNames.EmailPassword).Value; }
            set { SaveSettings(SettingNames.EmailPassword, value); }
        }

        public string EmailSubjectPrefix
        {
            get { return Database.Settings.Find((int)SettingNames.EmailSubjectPrefix).Value; }
            set { SaveSettings(SettingNames.EmailSubjectPrefix, value); }
        }

        public bool EmailSsl
        {
            get { return bool.Parse(Database.Settings.Find((int)SettingNames.EmailSsl).Value); }
            set { SaveSettings(SettingNames.EmailSsl, value.ToString()); }
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
            TimeZone = 6,

            EmailFrom = 7,
            EmailSmtpAddress = 8,
            EmailSmtpPort = 9,
            EmailUserName = 10,
            EmailPassword = 11,
            EmailSubjectPrefix = 12,
            EmailSsl = 13,

        }
    }
}
