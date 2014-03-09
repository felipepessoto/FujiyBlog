using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FujiyBlog.Core.Dto;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostIndex
    {
        public IEnumerable<PostSummary> RecentPosts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}