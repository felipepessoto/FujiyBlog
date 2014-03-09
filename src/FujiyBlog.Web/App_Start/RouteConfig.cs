using FujiyBlog.Web.Infrastructure;
using System.Web.Mvc;
using System.Web.Routing;

namespace FujiyBlog.Web
{
    public class RouteConfig
    {
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
                new { controller = "Page", action = "Index" },
                new[] { "FujiyBlog.Web.Controllers" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "", action = "", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "FujiyBlog.Web.Controllers" });
        }
    }
}