using FujiyBlog.Core.DomainObjects;
using System.Collections.Generic;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminCategoriesList
    {
        public AdminCategoriesList()
        {
            NewCategory = new Category();
        }

        public IDictionary<Category, int> CategoriesPostCount { get; set; }
        public Category NewCategory { get; set; }
    }
}