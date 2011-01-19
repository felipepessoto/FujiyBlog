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

            routes.MapRoute("Home", "", new { controller = "Blog", action = "Index" });
            routes.MapRoute("PostDetail", "{*PostSlug}", new { controller = "Post", action = "Details" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ConfigureUnity();
        }

        private static void ConfigureUnity()
        {
            var container = new UnityContainer();
            container.RegisterType<FujiyBlogDatabase, FujiyBlogDatabase>(new InjectionMember[]
                                                                             {
                                                                                 new InjectionConstructor(ConfigurationManager.ConnectionStrings["FujiyBlog"].ConnectionString)
                                                                             });
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IPostRepository, PostRepository>(); 
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}