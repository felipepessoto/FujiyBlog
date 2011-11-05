using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
                bool match = DependencyResolver.Current.GetService<FujiyBlogDatabase>().Pages.WhereHaveRoles().Where(x => x.IsFrontPage).Select(x => (string)null).FirstOrDefault() == null;
                return match;
            }

            return true;
        }
    }
}