using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Post
    {
        public virtual int Id { get; set; }

        [Required, StringLength(200)]
        public virtual string Title { get; set; }

        [StringLength(500)]
        public virtual string Description { get; set; }

        [Required, StringLength(200)]
        public virtual string Slug { get; set; }

        [Required]
        public virtual string Content { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime LastModificationDate { get; set; }

        public virtual DateTime PublicationDate { get; set; }

        public virtual bool IsPublished { get; set; }

        public virtual bool IsCommentEnabled { get; set; }

        public virtual bool IsDeleted { get; set; }

        [Required]
        public virtual User Author { get; set; }

        public virtual ICollection<PostComment> Comments { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public bool IsPublic
        {
            get { return IsPublished && !IsDeleted && PublicationDate < DateTime.Now; }
        }
    }
}
