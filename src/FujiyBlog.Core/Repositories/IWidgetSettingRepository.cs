using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IWidgetSettingRepository
    {
        IEnumerable<WidgetSetting> GetWidgetSettings(string zoneName);
    }
}