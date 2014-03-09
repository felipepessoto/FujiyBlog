using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Core.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool IsInRole(this IPrincipal principal, Role role)
        {
            if (principal.Identity.IsAuthenticated)
            {
                return principal.IsInRole(role.ToString());
            }

            return GetAnonymousRoles(role);
        }

        public static bool GetAnonymousRoles(Role role)
        {
            if (HttpContext.Current.Items["AnonymousRoleGroup"] == null)
            {
                RoleGroup roleGroup = CacheHelper.FromCacheOrExecute(() => DependencyResolver.Current.GetService<FujiyBlogDatabase>().RoleGroups.AsNoTracking().Single(x => x.Name == "Anonymous"), key: "FujiyBlog.Core.Extensions.PrincipalExtensions.GetAnonymousRoles", condition: !HttpContext.Current.User.Identity.IsAuthenticated);
                HttpContext.Current.Items["AnonymousRoleGroup"] = roleGroup;
            }

            return ((RoleGroup)HttpContext.Current.Items["AnonymousRoleGroup"]).Roles.Any(x => x == role);
        }
    }
}