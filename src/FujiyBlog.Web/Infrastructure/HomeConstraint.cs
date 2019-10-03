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
            if (httpContext != null)
            {
                string controllerName = values["controller"] as string;
                bool isPageController = string.Equals(controllerName, "Page", StringComparison.OrdinalIgnoreCase);
                bool isPostController = string.Equals(controllerName, "Post", StringComparison.OrdinalIgnoreCase);

                if ((isPageController || isPostController) && string.Equals(values["action"] as string, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    string cacheKey = "FujiyBlog.Web.Infrastructure.HomeConstraint.Match" + " as " + httpContext.User.Identity.Name;
                    var db = httpContext.RequestServices.GetRequiredService<FujiyBlogDatabase>();
                    bool hasFrontPage = CacheHelper.FromCacheOrExecute(db, () => db.Pages.WhereHaveRoles(httpContext).Any(x => x.IsFrontPage), cacheKey);
                    return hasFrontPage == isPageController;
                }

                return true;
            }
            else
            {
                return true;
            }
        }
    }
}