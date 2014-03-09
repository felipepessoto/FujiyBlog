using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class PostConfiguration : EntityTypeConfiguration<Post>
    {
        public PostConfiguration()
        {
            Property(b => b.Content).IsMaxLength();
            
        }
    }
}
