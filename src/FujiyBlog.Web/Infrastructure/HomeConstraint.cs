using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;

namespace FujiyBlog.Web.Infrastructure
{
    public class HomeConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (string.Equals(values["controller"] as string, MVC.Post.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(values["action"] as string, MVC.Post.ActionNames.Index, StringComparison.OrdinalIgnoreCase))
            {
                string cacheKey = "FujiyBlog.Web.Infrastructure.HomeConstraint.Match" + " as " + HttpContext.Current.User.Identity.Name;
                bool match = CacheHelper.FromCacheOrExecute(() => !DependencyResolver.Current.GetService<FujiyBlogDatabase>().Pages.WhereHaveRoles().Any(x => x.IsFrontPage), cacheKey);
                return match;
            }

            return true;
        }
    }
}