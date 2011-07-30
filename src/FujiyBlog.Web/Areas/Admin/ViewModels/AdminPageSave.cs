using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPageSave
    {
        public int? Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(500), AllowHtml]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        public DateTime PublicationDate { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Front Page")]
        public bool IsFrontPage { get; set; }

        [Display(Name = "Parent Page")]
        public int? ParentId { get; set; }

        [Display(Name = "Show In List")]
        public bool ShowInList { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }
    }
}