using System.Web.Mvc;
using System.Web.UI;

namespace FujiyBlog.Web.Infrastructure
{
    public class NonAuthenticatedOnlyCacheAttribute : OutputCacheAttribute
    {
        private OutputCacheLocation? originalLocation;

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                originalLocation = originalLocation ?? Location;
                Location = OutputCacheLocation.None;
            }
            else
            {
                Location = originalLocation ?? Location;
            }

            base.OnResultExecuting(filterContext);
        }
    }
}