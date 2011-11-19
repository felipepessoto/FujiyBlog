using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Infrastructure.AutoMapper;
using System.Diagnostics;

namespace FujiyBlog.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SetCultureAttribute());
            filters.Add(new ThemeAttribute());
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
            RegisterErrorTrace();

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

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            using (DependencyResolver.Current as IDisposable)
            {
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                CheckRollingTrace();
                Trace.WriteLine("-----------------------------------------------");
                Trace.WriteLine("Date: " + DateTime.UtcNow.ToString("u"));
                Trace.WriteLine(error.ToString());
                Trace.WriteLine("-----------------------------------------------");
                Trace.WriteLine(null);
            }
        }

        private static string lastTraceLogFile;
        private void RegisterErrorTrace()
        {
            Trace.Listeners.Clear();
            lastTraceLogFile = Server.MapPath("~/App_Data/FujiyBlog" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
            Trace.Listeners.Add(new TextWriterTraceListener(lastTraceLogFile));
        }

        private void CheckRollingTrace()
        {
            string logPath = Server.MapPath("~/App_Data/FujiyBlog" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");

            if (lastTraceLogFile != logPath)
            {
                foreach (TraceListener traceListener in Trace.Listeners.OfType<TraceListener>().ToArray())
                {
                    Trace.Listeners.Remove(traceListener);
                    traceListener.Dispose();
                }
                lastTraceLogFile = logPath;
                Trace.Listeners.Add(new TextWriterTraceListener(logPath));
            }
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            string result = string.Empty;

            foreach (string s in arg.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.ToLower() == "user")
                {
                    result += "user:" + context.User.Identity.Name;
                }
                if (s.ToLower() == "lastcache")
                {
                    result += "lastcache:" + DependencyResolver.Current.GetService<FujiyBlogDatabase>().LastCache;
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return base.GetVaryByCustomString(context, arg);
        }
    }
}