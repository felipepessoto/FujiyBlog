using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Web.Infrastructure;
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

            routes.MapRoute("Home", "", MVC.Post.Index());
            routes.MapRoute("PostDetailId", "postid/{Id}", MVC.Post.DetailsById());
            routes.MapRoute("PostDetail", "posts/{*PostSlug}", MVC.Post.Details());
            routes.MapRoute("TagHome", "tags/{tag}", MVC.Post.Tag());
            routes.MapRoute("CategoryHome", "category/{category}", MVC.Post.Category());
            routes.MapRoute("Archive", "archive", MVC.Post.Archive());

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "", action = "", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DependencyResolver.SetResolver(new UnityDependencyResolver());
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            using (DependencyResolver.Current as IDisposable) ;
        }
    }
}