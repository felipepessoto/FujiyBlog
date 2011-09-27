using System.Security.Principal;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Infrastructure
{
    public static class PrincipalExtensions
    {
        public static bool IsInRole(this IPrincipal principal, Permission permission)
        {
            return principal.IsInRole(permission.ToString());
        }
    }
}