using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class WidgetSettingConfiguration : EntityTypeConfiguration<WidgetSetting>
    {
        public WidgetSettingConfiguration()
        {
            Property(b => b.Name).IsUnicode(false);
            Property(b => b.WidgetZone).IsUnicode(false);
            Property(b => b.Settings).IsUnicode(false);
        }
    }
}
