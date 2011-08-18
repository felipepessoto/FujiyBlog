using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.ViewModels
{
    public class SearchResult
    {
        public IEnumerable<Post> Posts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}