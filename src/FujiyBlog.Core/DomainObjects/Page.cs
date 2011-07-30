using System;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Page
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

        public string Content { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModificationDate { get; set; }

        [Display(Name = "Publication Date")]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Front Page")]
        public bool IsFrontPage { get; set; }

        public Page Parent { get; set; }
        public int? ParentId { get; set; }

        [Display(Name = "Show In List")]
        public bool ShowInList { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }
    }
}
