using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FujiyBlog.Core.DomainObjects
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime LastModificationDate { get; set; }
        public virtual User Author { get; set; }
        public virtual bool IsPublished { get; set; }
        public virtual bool IsCommentEnabled { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual ICollection<PostComment> Comments { get; set; }
    }
}
