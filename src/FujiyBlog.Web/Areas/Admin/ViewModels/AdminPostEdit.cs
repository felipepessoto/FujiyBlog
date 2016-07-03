using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminPostEdit
    {
        public AdminPostSave Post { get; set; }
        public string AllTagsJson { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        public IEnumerable<SelectListItem> Authors { get; set; }
    }
}