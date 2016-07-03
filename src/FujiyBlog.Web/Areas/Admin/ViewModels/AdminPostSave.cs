using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostSave
    {
        public AdminPostSave()
        {
        }

        public AdminPostSave(Post post, DateTimeUtil dateTimeUtil)
        {
            if (post.Id > 0)
            {
                Id = post.Id;
            }

            Tags = string.Join(", ", post.PostTags.Select(x => x.Tag.Name));
            Categories = post.PostCategories.Select(x => x.Category);//TODO Maybe should I remove this property and just use SelectedCategories

            Title = post.Title;
            AuthorId = post.Author.Id;
            Description = post.Description;
            Slug = post.Slug;
            Content = post.Content;
            ImageUrl = post.ImageUrl;
            PublicationDate = dateTimeUtil.ConvertUtcToMyTimeZone(post.PublicationDate);
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
        public string AuthorId { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; }

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

        public void FillPost(Post post, DateTimeUtil dateTimeUtil)
        {
            post.Id = Id.GetValueOrDefault();
            post.Title = Title;
            post.Description = Description;
            post.Slug = Slug;
            post.Content = Content;
            post.ImageUrl = ImageUrl;
            post.PublicationDate = dateTimeUtil.ConvertMyTimeZoneToUtc(PublicationDate);
            post.IsPublished = IsPublished;
            post.IsCommentEnabled = IsCommentEnabled;
        }
    }
}