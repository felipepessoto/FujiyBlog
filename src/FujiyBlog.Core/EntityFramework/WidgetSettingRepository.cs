using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
{
    public class WidgetSettingRepository : RepositoryBase<WidgetSetting>
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
            return Database.WidgetSettings.Where(x => x.WidgetZone == widgetZone).OrderBy(x => x.Position).ToList();
        }
    }
}
