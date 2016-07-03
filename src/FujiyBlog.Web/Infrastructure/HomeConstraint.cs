using FujiyBlog.Core;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace FujiyBlog.Web.Infrastructure
{
    public class HomeConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (string.Equals(values["controller"] as string, "Post", StringComparison.OrdinalIgnoreCase) && string.Equals(values["action"] as string, "Index", StringComparison.OrdinalIgnoreCase))
            {
                string cacheKey = "FujiyBlog.Web.Infrastructure.HomeConstraint.Match" + " as " + httpContext.User.Identity.Name;
                var db = httpContext.RequestServices.GetRequiredService<FujiyBlogDatabase>();
                bool match = CacheHelper.FromCacheOrExecute(db, () => !db.Pages.WhereHaveRoles(httpContext).Any(x => x.IsFrontPage), cacheKey);
                return match;
            }

            return true;
        }
    }
}