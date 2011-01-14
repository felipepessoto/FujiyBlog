using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(b => b.Email).IsUnicode(false).HasMaxLength(50).IsRequired();
            Property(b => b.Password).IsUnicode(false).HasMaxLength(50).IsRequired();
        }
    }
}
