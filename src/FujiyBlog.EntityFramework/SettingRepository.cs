using System;
using FujiyBlog.Core.DomainObjects;
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
            get { return int.Parse(Database.Settings.Find("MinRequiredPasswordLength").Value); }
        }

        public int PostsPerPage
        {
            get { return int.Parse(Database.Settings.Find("PostsPerPage").Value); }
        }

        public string BlogName
        {
            get { return Database.Settings.Find("BlogName ").Value; }
        }

        public string BlogDescription
        {
            get { return Database.Settings.Find("BlogDescription ").Value; }
        }
    }
}
