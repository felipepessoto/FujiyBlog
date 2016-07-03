using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPageSave
    {
        public AdminPageSave()
        {
        }

        public AdminPageSave(Page page, DateTimeUtil dateTimeUtil)
        {
            if (page.Id > 0)
            {
                Id = page.Id;
            }
            Title = page.Title;
            AuthorId = page.Author.Id;
            Description = page.Description;
            Slug = page.Slug;
            Content = page.Content;
            Keywords = page.Keywords;
            PublicationDate = dateTimeUtil.ConvertUtcToMyTimeZone(page.PublicationDate);
            IsPublished = page.IsPublished;
            IsFrontPage = page.IsFrontPage;
            ParentId = page.ParentId;
            ShowInList = page.ShowInList;
        }

        public int? Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "Author")]
        public string AuthorId { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

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

        public void FillPage(Page page, DateTimeUtil dateTimeUtil)
        {
            page.Id = Id.GetValueOrDefault();
            page.Title = Title;
            page.Description = Description;
            page.Slug = Slug;
            page.Content = Content;
            page.Keywords = Keywords;
            page.PublicationDate = dateTimeUtil.ConvertMyTimeZoneToUtc(PublicationDate);
            page.IsPublished = IsPublished;
            page.IsFrontPage = IsFrontPage;
            page.ParentId = ParentId;
            page.ShowInList = ShowInList;
        }
    }
}