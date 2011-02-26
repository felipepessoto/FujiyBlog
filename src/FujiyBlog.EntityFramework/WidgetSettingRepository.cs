using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class WidgetSettingRepository : RepositoryBase<WidgetSetting>, IWidgetSettingRepository
    {
        public WidgetSettingRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public WidgetSetting GetWidgetSetting(int id)
        {
            return Database.WidgetSettings.Single(x => x.Id == id);
        }

        public IEnumerable<WidgetSetting> GetWidgetSettings(string widgetZone)
        {
            return Database.WidgetSettings.Where(x => x.WidgetZone == widgetZone).ToList();
        }
    }
}
