using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasMany(b => b.AuthoredPostComments).WithOptional(b => b.Author);
            HasMany(b => b.ModeratedPostComments).WithOptional(b => b.ModeratedBy);
        }
    }
}
