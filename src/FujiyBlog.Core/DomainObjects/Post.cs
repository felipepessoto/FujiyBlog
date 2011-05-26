using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Post
    {
        public Post()
        {
            Comments = new List<PostComment>();
            Categories = new List<Category>();
            Tags = new List<Tag>();
        }

        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModificationDate { get; set; }

        public DateTime PublicationDate { get; set; }

        public bool IsPublished { get; set; }

        public bool IsCommentEnabled { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public virtual User Author { get; set; }

        public virtual ICollection<PostComment> Comments { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
