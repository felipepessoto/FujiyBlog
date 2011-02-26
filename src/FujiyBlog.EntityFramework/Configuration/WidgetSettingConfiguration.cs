using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class WidgetSettingConfiguration : EntityTypeConfiguration<WidgetSetting>
    {
        public WidgetSettingConfiguration()
        {
            Property(b => b.Name).IsUnicode(false);
            Property(b => b.WidgetZone).IsUnicode(false);
        }
    }
}
