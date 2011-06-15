using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminCommentIndex
    {
        public IEnumerable<PostComment> Comments { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}