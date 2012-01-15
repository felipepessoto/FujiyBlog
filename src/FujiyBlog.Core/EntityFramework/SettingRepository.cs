using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using System.ComponentModel;

namespace FujiyBlog.Core.EntityFramework
{
    public class SettingRepository
    {
        private readonly FujiyBlogDatabase database;
        private readonly Dictionary<int, string> settings;

        public SettingRepository(FujiyBlogDatabase database)
        {
            this.database = database;
            settings = CacheHelper.FromCacheOrExecute(() => database.Settings.ToDictionary(x => x.Id, x => x.Value), "FujiyBlog.Core.EntityFramework.SettingRepository.settings");
        }

        public int  MinRequiredPasswordLength
        {
            get { return int.Parse(LoadSetting(SettingNames.MinRequiredPasswordLength)); }
            set { SaveSetting(SettingNames.MinRequiredPasswordLength, value.ToString()); }
        }

        public int PostsPerPage
        {
            get { return int.Parse(LoadSetting(SettingNames.PostsPerPage)); }
            set { SaveSetting(SettingNames.PostsPerPage, value.ToString()); }
        }

        public string BlogName
        {
            get { return LoadSetting(SettingNames.BlogName); }
            set { SaveSetting(SettingNames.BlogName, value); }
        }

        public string BlogDescription
        {
            get { return LoadSetting(SettingNames.BlogDescription); }
            set { SaveSetting(SettingNames.BlogDescription, value); }
        }

        public string Theme
        {
            get { return LoadSetting(SettingNames.Theme); }
            set { SaveSetting(SettingNames.Theme, value); }
        }

        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(LoadSetting(SettingNames.TimeZone)); }
            set { SaveSetting(SettingNames.TimeZone, value.Id); }
        }




        public string EmailTo
        {
            get { return LoadSetting(SettingNames.EmailTo); }
            set { SaveSetting(SettingNames.EmailTo, value); }
        }

        public string EmailSubjectPrefix
        {
            get { return LoadSetting(SettingNames.EmailSubjectPrefix); }
            set { SaveSetting(SettingNames.EmailSubjectPrefix, value); }
        }

        public string SmtpAddress
        {
            get { return LoadSetting(SettingNames.SmtpAddress); }
            set { SaveSetting(SettingNames.SmtpAddress, value); }
        }

        public int SmtpPort
        {
            get { return int.Parse(LoadSetting(SettingNames.SmtpPort)); }
            set { SaveSetting(SettingNames.SmtpPort, value.ToString()); }
        }

        public string SmtpUserName
        {
            get { return LoadSetting(SettingNames.SmtpUserName); }
            set { SaveSetting(SettingNames.SmtpUserName, value); }
        }

        public string SmtpPassword
        {
            get { return LoadSetting(SettingNames.SmtpPassword); }
            set { SaveSetting(SettingNames.SmtpPassword, value); }
        }

        public bool SmtpSsl
        {
            get { return bool.Parse(LoadSetting(SettingNames.SmtpSsl)); }
            set { SaveSetting(SettingNames.SmtpSsl, value.ToString()); }
        }




        public bool EnableComments
        {
            get { return bool.Parse(LoadSetting(SettingNames.EnableComments)); }
            set { SaveSetting(SettingNames.EnableComments, value.ToString()); }
        }

        public bool ModerateComments
        {
            get { return bool.Parse(LoadSetting(SettingNames.ModerateComments)); }
            set { SaveSetting(SettingNames.ModerateComments, value.ToString()); }
        }

        public bool EnableNestedComments
        {
            get { return bool.Parse(LoadSetting(SettingNames.EnableNestedComments)); }
            set { SaveSetting(SettingNames.EnableNestedComments, value.ToString()); }
        }

        public int? CloseCommentsAfterDays
        {
            get
            {
                string value = LoadSetting(SettingNames.CloseCommentsAfterDays);
                if (value == null)
                    return null;
                return int.Parse(value);
            }
            set { SaveSetting(SettingNames.CloseCommentsAfterDays, value.HasValue ? value.ToString() : null); }
        }

        public int CommentsPerPage
        {
            get { return int.Parse(LoadSetting(SettingNames.CommentsPerPage)); }
            set { SaveSetting(SettingNames.CommentsPerPage, value.ToString()); }
        }

        public string CommentsAvatar
        {
            get { return LoadSetting(SettingNames.CommentsAvatar); }
            set { SaveSetting(SettingNames.CommentsAvatar, value); }
        }

        public string Culture
        {
            get { return LoadSetting(SettingNames.Culture); }
            set { SaveSetting(SettingNames.Culture, value); }
        }




        public bool EnableFacebookLikePosts
        {
            get { return bool.Parse(LoadSetting(SettingNames.EnableFacebookLikePosts)); }
            set { SaveSetting(SettingNames.EnableFacebookLikePosts, value.ToString()); }
        }

        public bool EnableGooglePlusOnePosts
        {
            get { return bool.Parse(LoadSetting(SettingNames.EnableGooglePlusOnePosts)); }
            set { SaveSetting(SettingNames.EnableGooglePlusOnePosts, value.ToString()); }
        }

        public bool EnableTwitterSharePosts
        {
            get { return bool.Parse(LoadSetting(SettingNames.EnableTwitterSharePosts)); }
            set { SaveSetting(SettingNames.EnableTwitterSharePosts, value.ToString()); }
        }



        //public long LastDatabaseChange
        //{
        //    get { return long.Parse(LoadSetting(SettingNames.LastDatabaseChange)); }
        //    set { SaveSetting(SettingNames.LastDatabaseChange, value.ToString()); }
        //}
        
        public string CustomCode
        {
            get { return LoadSetting(SettingNames.CustomCode); }
            set { SaveSetting(SettingNames.CustomCode, value); }
        }

        public string AlternateFeedUrl
        {
            get { return LoadSetting(SettingNames.AlternateFeedUrl); }
            set { SaveSetting(SettingNames.AlternateFeedUrl, value); }
        }

        public int ItemsShownInFeed
        {
            get
            {
                int itemsShownInFeed;
                int.TryParse(LoadSetting(SettingNames.ItemsShownInFeed), out itemsShownInFeed);
                return itemsShownInFeed;
            }
            set { SaveSetting(SettingNames.ItemsShownInFeed, value.ToString()); }
        }

        public string DefaultFeedOutput
        {
            get { return LoadSetting(SettingNames.DefaultFeedOutput); }
            set { SaveSetting(SettingNames.DefaultFeedOutput, value); }
        }

        public string FacebookAdminIds
        {
            get { return LoadSetting(SettingNames.FacebookAdminIds); }
            set { SaveSetting(SettingNames.FacebookAdminIds, value); }
        }

        public string FacebookAppId
        {
            get { return LoadSetting(SettingNames.FacebookAppId); }
            set { SaveSetting(SettingNames.FacebookAppId, value); }
        }

        public string OpenGraphImageUrl
        {
            get { return LoadSetting(SettingNames.OpenGraphImageUrl); }
            set { SaveSetting(SettingNames.OpenGraphImageUrl, value); }
        }

        private string LoadSetting(SettingNames setting)
        {
            string value;
            if (settings.TryGetValue((int)setting, out value))
            {
                return value;
            }
            return GetSettingNameDefaultValue(setting);
        }

        private void SaveSetting(SettingNames settingName, string value)
        {
            int settingId = (int) settingName;
            Setting setting = database.Settings.Find(settingId);

            if (setting == null)
            {
                setting = new Setting {Id = settingId, Description = settingName.ToString()};
                database.Settings.Add(setting);
            }

            setting.Value = value;
            database.SaveChanges();
        }

        private string GetSettingNameDefaultValue(SettingNames settingName)
        {
            MemberInfo enumValue = typeof (SettingNames).GetMember(settingName.ToString())[0];
            DefaultValueAttribute defaultValue = (DefaultValueAttribute) enumValue.GetCustomAttributes(typeof (DefaultValueAttribute), false).FirstOrDefault();
            if (defaultValue != null)
                return defaultValue.Value.ToString();
            return null;
        }

        private enum SettingNames
        {
            MinRequiredPasswordLength = 1,
            PostsPerPage = 2,
            BlogName = 3,
            BlogDescription = 4,
            Theme = 5,
            TimeZone = 6,

            EmailTo = 7,
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

            Culture = 20,

            EnableFacebookLikePosts = 21,
            EnableGooglePlusOnePosts = 22,
            EnableTwitterSharePosts = 23,

            LastDatabaseChange = 24,

            CustomCode = 25,

            AlternateFeedUrl = 26,
            ItemsShownInFeed = 27,
            DefaultFeedOutput = 28,

            [Description("Comma-separated list of the user IDs or usernames of the Facebook accounts")]
            FacebookAdminIds = 29,

            [Description("Facebook application ID")]
            FacebookAppId = 30,

            [Description("")]
            OpenGraphImageUrl = 31,
        }
    }
}
