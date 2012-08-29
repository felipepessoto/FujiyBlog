using System.Web;
using System.Web.Mvc;
using FujiyBlog.Web.Infrastructure;

namespace FujiyBlog.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ProfilingAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SetCultureAttribute());
            filters.Add(new ThemeAttribute());
        }
    }
}