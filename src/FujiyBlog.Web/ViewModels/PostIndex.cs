using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.ViewModels
{
    public class PostIndex
    {
        public IEnumerable<Post> RecentPosts { get; set; }
        public int TotalPosts { get; set; }
        public int PostsPerPage { get; set; }
    }
}