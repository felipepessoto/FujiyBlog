﻿using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.EntityFramework
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
        }

        public int PostsPerPage
        {
            get { return int.Parse(Database.Settings.Find((int)SettingNames.PostsPerPage).Value); }
        }

        public string BlogName
        {
            get { return Database.Settings.Find((int)SettingNames.BlogName).Value; }
        }

        public string BlogDescription
        {
            get { return Database.Settings.Find((int)SettingNames.BlogDescription).Value; }
        }

        private enum SettingNames
        {
            MinRequiredPasswordLength = 1,
            PostsPerPage = 2,
            BlogName = 3,
            BlogDescription = 4
        }
    }
}