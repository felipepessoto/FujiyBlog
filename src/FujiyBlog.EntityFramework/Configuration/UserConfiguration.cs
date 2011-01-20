using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(b => b.Login).IsUnicode(false);
            Property(b => b.Email).IsUnicode(false);
            Property(b => b.Password).IsUnicode(false);
            Property(b => b.DisplayName).IsUnicode(false);
            Property(b => b.FullName).IsUnicode(false);
            Property(b => b.Location).IsUnicode(false);
            Property(b => b.About).IsUnicode(false);
        }
    }
}
