using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.ViewModels
{
    public class PostArchive
    {
        public IEnumerable<Post> AllPosts { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        public IEnumerable<Post> UncategorizedPosts { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
    }
}