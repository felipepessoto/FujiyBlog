using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminTagsList
    {
        public IDictionary<Tag, int> TagsPostCount { get; set; }
    }
}