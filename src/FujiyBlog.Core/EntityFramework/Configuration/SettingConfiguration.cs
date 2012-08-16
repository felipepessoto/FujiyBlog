using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class SettingConfiguration : EntityTypeConfiguration<Setting>
    {
        public SettingConfiguration()
        {
            Property(b => b.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(b => b.Value).IsMaxLength();
        }
    }
}
