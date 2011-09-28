using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Web.Infrastructure
{
    public static class PrincipalExtensions
    {
        public static bool IsInRole(this IPrincipal principal, Permission permission)
        {
            if (principal.Identity.IsAuthenticated)
            {
                return principal.IsInRole(permission.ToString());
            }

            if(HttpContext.Current.Items["AnonymousPermissionGroup"] == null)
            {
                FujiyBlogDatabase db = DependencyResolver.Current.GetService<FujiyBlogDatabase>();
                PermissionGroup permissionGroup = db.PermissionGroups.AsNoTracking().Single(x => x.Name == "Anonymous");
                HttpContext.Current.Items["AnonymousPermissionGroup"] = permissionGroup;
            }

            return ((PermissionGroup)HttpContext.Current.Items["AnonymousPermissionGroup"]).Permissions.Any(x=>x == permission);
        }
    }
}