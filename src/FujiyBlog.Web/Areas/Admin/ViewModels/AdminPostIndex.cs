using FujiyBlog.Core.Dto;
using System.Collections.Generic;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostIndex
    {
        public IEnumerable<PostSummary> RecentPosts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool? Published { get; internal set; }
    }
}