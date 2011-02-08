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

        public Setting  MinRequiredPasswordLength
        {
            get { return Database.Settings.Find("MinRequiredPasswordLength"); }
        }
    }
}
