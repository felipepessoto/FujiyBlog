using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class SettingConfiguration : EntityTypeConfiguration<Setting>
    {
        public SettingConfiguration()
        {
            Property(b => b.Key).IsUnicode(false);
            Property(b => b.Description).IsUnicode(false);
            Property(b => b.Value).IsUnicode(false).IsMaxLength();
        }
    }
}
