using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class PostCommentConfiguration : EntityTypeConfiguration<PostComment>
    {
        public PostCommentConfiguration()
        {
            Property(b => b.Comment).IsMaxLength();
            HasOptional(t => t.ParentComment).WithMany(t => t.NestedComments);
        }
    }
}
