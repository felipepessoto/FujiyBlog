using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IWidgetSettingRepository : IRepository<WidgetSetting>
    {
        WidgetSetting GetWidgetSetting(int id);
        IEnumerable<WidgetSetting> GetWidgetSettings(string zoneName);
    }
}

