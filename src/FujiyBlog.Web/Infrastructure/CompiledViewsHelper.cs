using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Infrastructure
{
    internal class CompiledViewsHelper
    {
        public static List<Assembly> GetViewsAssemblies()
        {
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var precompiledViews = allAssemblies.Where(x => x.GetName().Name == "FujiyBlog.Web.PrecompiledViews").ToList();

            List<Assembly> assemblies =
                precompiledViews.Any()
                ? precompiledViews
                : allAssemblies.Where(x => x.GetName().Name.StartsWith("Microsoft") == false && x.GetName().Name.StartsWith("System") == false && x.GetName().Name.StartsWith("netstandard") == false && x.GetName().Name.StartsWith("mscorlib") == false).ToList();
            return assemblies;
        }

        public static List<Type> GetViewsTypes<TModel>()
        {
            List<Type> viewsTypes = GetAllViews();

            var result = viewsTypes.Where(type => type.IsSubclassOf(typeof(RazorPage<TModel>))).ToList();

            return result;
        }

        public static List<Type> GetAllViews()
        {
            List<Assembly> assemblies = GetViewsAssemblies();
            var viewsTypes = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(x => x.Name.StartsWith("_Views_")).ToList();
            return viewsTypes;
        }
    }
}
