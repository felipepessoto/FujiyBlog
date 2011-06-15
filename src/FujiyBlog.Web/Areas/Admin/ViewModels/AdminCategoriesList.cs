using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FujiyBlog.Core.DomainObjects;

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