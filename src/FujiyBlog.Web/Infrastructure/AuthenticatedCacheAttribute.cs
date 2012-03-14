using System.Web.Mvc;
using System.Web.UI;

namespace FujiyBlog.Web.Infrastructure
{
    public class AuthenticatedCacheAttribute : OutputCacheAttribute
    {
        public OutputCacheLocation AuthenticatedLocation { get; set; }
        public OutputCacheLocation AnonymousLocation { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Location = filterContext.HttpContext.User.Identity.IsAuthenticated ? AuthenticatedLocation : AnonymousLocation;
            base.OnResultExecuting(filterContext);
        }
    }
}