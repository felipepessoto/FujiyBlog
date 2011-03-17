using System.Collections.Generic;
using FujiyBlog.Core.Dto;

namespace FujiyBlog.Web.ViewModels
{
    public class PostIndex
    {
        public IEnumerable<PostSummary> RecentPosts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}