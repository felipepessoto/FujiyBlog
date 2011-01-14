using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework.Configuration
{
    public class PostCommentConfiguration : EntityTypeConfiguration<PostComment>
    {
        public PostCommentConfiguration()
        {
            Property(b => b.Comment).IsUnicode(false).HasMaxLength(200).IsRequired();
            HasRequired(x => x.Post);
        }
    }
}
