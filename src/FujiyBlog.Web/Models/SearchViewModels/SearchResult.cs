using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.ViewModels.SearchViewModels
{
    public class SearchResult
    {
        public IEnumerable<Post> Posts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Count { get; internal set; }
        public string Terms { get; internal set; }
    }
}