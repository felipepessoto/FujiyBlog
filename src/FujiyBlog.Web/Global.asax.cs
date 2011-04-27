using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Models;
using Microsoft.Practices.Unity;
using FujiyBlog.EntityFramework;
using System.Configuration;
using FujiyBlog.Core.Infrastructure;

namespace FujiyBlog.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", MVC.Post.Index(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("PostDetailId", "postid/{Id}", MVC.Post.DetailsById(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("PostDetail", "posts/{*PostSlug}", MVC.Post.Details(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("TagHome", "tags/{tag}", MVC.Post.Tag(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("CategoryHome", "category/{category}", MVC.Post.Category(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("AuthorHome", "author/{author}", MVC.Post.Author(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("Archive", "archive", MVC.Post.Archive(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("ArchiveByMonth", "archive/{year}/{month}", MVC.Post.ArchiveDate(), null, new { year = @"\d{4}", month = @"\d{1,2}" }, new[] { "FujiyBlog.Web.Controllers" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "", action = "", id = UrlParameter.Optional}, // Parameter defaults
                new[] {"FujiyBlog.Web.Controllers"});
        }

        protected void Application_Start()
        {
            foreach (IViewEngine viewEngine in ViewEngines.Engines.Where(x=> !(x is RazorViewEngine)).ToList())
            {
                ViewEngines.Engines.Remove(viewEngine);
            }

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DependencyResolver.SetResolver(new UnityDependencyResolver());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            RazorViewEngine engine = (RazorViewEngine) ViewEngines.Engines.Single();

            string themeName = Settings.SettingRepository.Theme;

            engine.MasterLocationFormats = new[]
                                               {
                                                   "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                   "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                   "~/Views/{1}/{0}.cshtml",
                                                   "~/Views/Shared/{0}.cshtml",
                                               };

            engine.ViewLocationFormats = new[]
                                             {
                                                 "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                 "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                  "~/Views/{1}/{0}.cshtml",
                                                   "~/Views/Shared/{0}.cshtml",
                                             };

            engine.PartialViewLocationFormats = new[]
                                                    {
                                                        "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                        "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                        "~/Views/{1}/{0}.cshtml",
                                                        "~/Views/Shared/{0}.cshtml",
                                                    };
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            using (DependencyResolver.Current as IDisposable)
            {
            }
        }
    }
}