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

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModificationDate { get; set; }

        [Display(Name = "Publication Date")]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Comments Enabled")]
        public bool IsCommentEnabled { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        [Required]
        public User Author { get; set; }

        public ICollection<PostComment> Comments { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
