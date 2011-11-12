using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Infrastructure.AutoMapper;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SetCultureAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("PostDetailId", "postid/{Id}", MVC.Post.DetailsById(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("PostDetail", "post/{*PostSlug}", MVC.Post.Details(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("PageById", "pageid/{Id}", MVC.Page.DetailsById(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("Page", "page/{*PageSlug}", MVC.Page.Details(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("TagHome", "tag/{tag}", MVC.Post.Tag(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("CategoryHome", "category/{category}", MVC.Post.Category(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("AuthorHome", "author/{author}", MVC.Post.Author(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("Archive", "archive", MVC.Post.Archive(), new[] { "FujiyBlog.Web.Controllers" });
            routes.MapRoute("ArchiveByMonth", "archive/{year}/{month}", MVC.Post.ArchiveDate(), null, new { year = @"\d{4}", month = @"\d{1,2}" }, new[] { "FujiyBlog.Web.Controllers" });

            routes.MapRoute(
                "HomePosts",
                "",
                new { controller = "Post", action = "Index" },
                new { controller = new HomeConstraint() },
                new[] { "FujiyBlog.Web.Controllers" });

            routes.MapRoute("BlogHome", "blog", new { controller = "Post", action = "Index" }, new[] { "FujiyBlog.Web.Controllers" });

            routes.MapRoute(
                "DefaultPage",
                "",
                new {controller = "Page", action = "Index"},
                new[] {"FujiyBlog.Web.Controllers"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "", action = "", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "FujiyBlog.Web.Controllers" });
        }

        protected void Application_Start()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            foreach (IViewEngine viewEngine in ViewEngines.Engines.Where(x=> !(x is RazorViewEngine)).ToList())
            {
                ViewEngines.Engines.Remove(viewEngine);
            }

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DependencyResolver.SetResolver(new UnityDependencyResolver());
            AutoMapperConfiguration.Configure();
        }

        public static string LastCache;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string lastCacheAtDb = DependencyResolver.Current.GetService<FujiyBlogDatabase>().Settings.Single(x => x.Id == 24).Value;
            if (lastCacheAtDb != LastCache)
            {
                LastCache = lastCacheAtDb;
                CacheHelper.ClearCache();
            }
            RegisterThemes();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            using (DependencyResolver.Current as IDisposable)
            {
            }
        }

        private void RegisterThemes()
        {
            RazorViewEngine engine = (RazorViewEngine)ViewEngines.Engines.Single();

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
    }
}