using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class WidgetSettingConfiguration : EntityTypeConfiguration<WidgetSetting>
    {
        public WidgetSettingConfiguration()
        {
            Property(b => b.Settings).IsMaxLength();
        }
    }
}
