using System;

namespace FujiyBlog.Core.EntityFramework
{
    public class SettingRepository
    {
        private readonly FujiyBlogDatabase database;

        public SettingRepository(FujiyBlogDatabase database)
        {
            this.database = database;
        }

        public int  MinRequiredPasswordLength
        {
            get { return int.Parse(database.Settings.Find((int)SettingNames.MinRequiredPasswordLength).Value); }
            set { SaveSettings(SettingNames.MinRequiredPasswordLength, value.ToString()); }
        }

        public int PostsPerPage
        {
            get { return int.Parse(database.Settings.Find((int)SettingNames.PostsPerPage).Value); }
            set { SaveSettings(SettingNames.PostsPerPage, value.ToString()); }
        }

        public string BlogName
        {
            get { return database.Settings.Find((int)SettingNames.BlogName).Value; }
            set { SaveSettings(SettingNames.BlogName, value); }
        }

        public string BlogDescription
        {
            get { return database.Settings.Find((int)SettingNames.BlogDescription).Value; }
            set { SaveSettings(SettingNames.BlogDescription, value); }
        }

        public string Theme
        {
            get { return database.Settings.Find((int)SettingNames.Theme).Value; }
            set { SaveSettings(SettingNames.Theme, value); }
        }

        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(database.Settings.Find((int)SettingNames.TimeZone).Value); }
            set { SaveSettings(SettingNames.TimeZone, value.Id); }
        }




        public string EmailFrom
        {
            get { return database.Settings.Find((int)SettingNames.EmailFrom).Value; }
            set { SaveSettings(SettingNames.EmailFrom, value); }
        }

        public string EmailSubjectPrefix
        {
            get { return database.Settings.Find((int)SettingNames.EmailSubjectPrefix).Value; }
            set { SaveSettings(SettingNames.EmailSubjectPrefix, value); }
        }

        public string SmtpAddress
        {
            get { return database.Settings.Find((int)SettingNames.SmtpAddress).Value; }
            set { SaveSettings(SettingNames.SmtpAddress, value); }
        }

        public int SmtpPort
        {
            get { return int.Parse(database.Settings.Find((int)SettingNames.SmtpPort).Value); }
            set { SaveSettings(SettingNames.SmtpPort, value.ToString()); }
        }

        public string SmtpUserName
        {
            get { return database.Settings.Find((int)SettingNames.SmtpUserName).Value; }
            set { SaveSettings(SettingNames.SmtpUserName, value); }
        }

        public string SmtpPassword
        {
            get { return database.Settings.Find((int)SettingNames.SmtpPassword).Value; }
            set { SaveSettings(SettingNames.SmtpPassword, value); }
        }

        public bool SmtpSsl
        {
            get { return bool.Parse(database.Settings.Find((int)SettingNames.SmtpSsl).Value); }
            set { SaveSettings(SettingNames.SmtpSsl, value.ToString()); }
        }




        public bool EnableComments
        {
            get { return bool.Parse(database.Settings.Find((int)SettingNames.EnableComments).Value); }
            set { SaveSettings(SettingNames.EnableComments, value.ToString()); }
        }

        public bool ModerateComments
        {
            get { return bool.Parse(database.Settings.Find((int)SettingNames.ModerateComments).Value); }
            set { SaveSettings(SettingNames.ModerateComments, value.ToString()); }
        }

        public bool EnableNestedComments
        {
            get { return bool.Parse(database.Settings.Find((int)SettingNames.EnableNestedComments).Value); }
            set { SaveSettings(SettingNames.EnableNestedComments, value.ToString()); }
        }

        public int? CloseCommentsAfterDays
        {
            get
            {
                string value = database.Settings.Find((int)SettingNames.CloseCommentsAfterDays).Value;
                if (value == null)
                    return null;
                return int.Parse(value);
            }
            set { SaveSettings(SettingNames.CloseCommentsAfterDays, value.HasValue ? value.ToString() : null); }
        }

        public int CommentsPerPage
        {
            get { return int.Parse(database.Settings.Find((int)SettingNames.CommentsPerPage).Value); }
            set { SaveSettings(SettingNames.CommentsPerPage, value.ToString()); }
        }

        public string CommentsAvatar
        {
            get { return database.Settings.Find((int)SettingNames.CommentsAvatar).Value; }
            set { SaveSettings(SettingNames.CommentsAvatar, value); }
        }

        private void SaveSettings(SettingNames settings, string value)
        {
            database.Settings.Find((int) settings).Value = value;
            database.SaveChanges();
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
            EmailSubjectPrefix = 8,
            SmtpAddress = 9,
            SmtpPort = 10,
            SmtpUserName = 11,
            SmtpPassword = 12,
            SmtpSsl = 13,

            EnableComments = 14,
            ModerateComments = 15,
            EnableNestedComments = 16,
            CloseCommentsAfterDays = 17,
            CommentsPerPage = 18,
            CommentsAvatar = 19,
        }
    }
}
