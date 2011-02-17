using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;

namespace FujiyBlog.Web.ViewModels
{
    public class PostIndex
    {
        public IEnumerable<PostSummary> RecentPosts { get; set; }
        public int TotalPosts { get; set; }
        public int PostsPerPage { get; set; }
    }
}