using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;

namespace FujiyBlog.Web.ViewModels
{
    public class PostArchive
    {
        public IEnumerable<PostSummary> AllPosts { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        public IEnumerable<PostSummary> UncategorizedPosts { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
    }
}