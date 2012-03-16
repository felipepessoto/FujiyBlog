using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class PostConfiguration : EntityTypeConfiguration<Post>
    {
        public PostConfiguration()
        {
            Property(b => b.Title).IsUnicode(false);
            Property(b => b.Description).IsUnicode(false);
            Property(b => b.Slug).IsUnicode(false);
            Property(b => b.Content).IsUnicode(false).IsMaxLength();
            Property(b => b.ImageUrl).IsUnicode(false);
        }
    }
}
