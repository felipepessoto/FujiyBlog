using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(b => b.Username).IsUnicode(false);
            Property(b => b.Email).IsUnicode(false);
            Property(b => b.Password).IsUnicode(false);
            Property(b => b.DisplayName).IsUnicode(false);
            Property(b => b.FullName).IsUnicode(false);
            Property(b => b.Location).IsUnicode(false);
            Property(b => b.About).IsUnicode(false);

            HasMany(b => b.AuthoredPostComments).WithOptional(b => b.Author);
            HasMany(b => b.ModeratedPostComments).WithOptional(b => b.ModeratedBy);
        }
    }
}
