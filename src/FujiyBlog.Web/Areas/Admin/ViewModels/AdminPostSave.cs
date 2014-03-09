using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Common;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostSave
    {
        public AdminPostSave()
        {
        }

        public AdminPostSave(Post post)
        {
            if (post.Id > 0)
            {
                Id = post.Id;
            }

            Tags = string.Join(", ", post.Tags.Select(x => x.Name));
            Categories = post.Categories;//TODO Maybe should I remove this property and just use SelectedCategories

            Title = post.Title;
            AuthorId = post.Author.Id;
            Description = post.Description;
            Slug = post.Slug;
            Content = post.Content;
            ImageUrl = post.ImageUrl;
            PublicationDate = DateTimeUtil.ConvertUtcToMyTimeZone(post.PublicationDate);
            IsPublished = post.IsPublished;
            IsCommentEnabled = post.IsCommentEnabled;
        }

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

        [StringLength(255)]
        public string ImageUrl { get; set; }

        [UIHint("DateTimePicker")]
        [DisplayName("Publish Date")]
        public DateTime PublicationDate { get; set; }

        [DisplayName("Publish")]
        public bool IsPublished { get; set; }

        [DisplayName("Enable Comments")]
        public bool IsCommentEnabled { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public void FillPost(Post post)
        {
            post.Id = Id.GetValueOrDefault();
            post.Title = Title;
            post.Description = Description;
            post.Slug = Slug;
            post.Content = Content;
            post.ImageUrl = ImageUrl;
            post.PublicationDate = DateTimeUtil.ConvertMyTimeZoneToUtc(PublicationDate);
            post.IsPublished = IsPublished;
            post.IsCommentEnabled = IsCommentEnabled;
        }
    }
}