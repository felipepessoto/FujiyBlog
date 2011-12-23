using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPageSave
    {
        public int? Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "Author")]
        public int? AuthorId { get; set; }

        [StringLength(500), AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Keywords { get; set; }

        [UIHint("DateTimePicker")]
        [DisplayName("Publish Date")]
        public DateTime PublicationDate { get; set; }

        [DisplayName("Publish")]
        public bool IsPublished { get; set; }

        [Display(Name = "Front Page")]
        public bool IsFrontPage { get; set; }

        [Display(Name = "Parent Page")]
        public int? ParentId { get; set; }

        [Display(Name = "Show In List")]
        public bool ShowInList { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }
        public IEnumerable<SelectListItem> Authors { get; set; }
    }
}