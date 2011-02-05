using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.EntityFramework
{
    public class SettingRepository :  RepositoryBase<Post>, ISettingRepository
    {
        public SettingRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public Setting GetSetting(string key)
        {
            return Database.Settings.Find(key);
        }
    }
}
