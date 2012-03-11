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
            if (string.Compare(values["controller"] as string, MVC.Post.Name, true) == 0 && string.Compare(values["action"] as string, MVC.Post.ActionNames.Index, true) == 0)
            {
                string cacheKey = "FujiyBlog.Web.Infrastructure.HomeConstraint.Match";
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    cacheKey += " as " + HttpContext.Current.User.Identity.Name;
                }
                bool match = CacheHelper.FromCacheOrExecute(() => DependencyResolver.Current.GetService<FujiyBlogDatabase>().Pages.WhereHaveRoles().Where(x => x.IsFrontPage).Select(x => (string)null).FirstOrDefault() == null, cacheKey);
                return match;
            }

            return true;
        }
    }
}