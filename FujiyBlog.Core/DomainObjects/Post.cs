using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Post
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime LastModificationDate { get; set; }

        public virtual DateTime PublishedDate { get; set; }

        public virtual bool IsPublished { get; set; }

        public virtual bool IsCommentEnabled { get; set; }

        public virtual bool IsDeleted { get; set; }

        [Required]
        public virtual User Author { get; set; }

        public virtual ICollection<PostComment> Comments { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
