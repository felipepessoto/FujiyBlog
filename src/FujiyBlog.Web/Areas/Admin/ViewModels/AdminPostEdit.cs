using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostEdit
    {
        public AdminPostSave Post { get; set; }
        public IEnumerable<Tag> AllTags { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
    }
}