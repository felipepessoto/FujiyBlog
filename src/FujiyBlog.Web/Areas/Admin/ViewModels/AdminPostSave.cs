using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostSave
    {
        public int? Id { get; set; }

        [DataType(DataType.MultilineText)]
        public string Tags { get; set; }

        public IEnumerable<int> SelectedCategories { get; set; }

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

        [UIHint("DateTimePicker")]
        [DisplayName("Publish Date")]
        public DateTime PublicationDate { get; set; }

        [DisplayName("Publish")]
        public bool IsPublished { get; set; }

        [DisplayName("Enable Comments")]
        public bool IsCommentEnabled { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}