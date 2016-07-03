using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool UserHasClaimPermission(this HttpContext context, PermissionClaims permission)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.HasClaim(CustomClaimTypes.Permission, permission.ToString());
            }

            return GetAnonymousRoles(context, permission).Result;
        }
        

        private static async Task<bool> GetAnonymousRoles(HttpContext httpContext, PermissionClaims permission)
        {
            //TODO validar performance
            string roleName = permission.ToString();
            var roleManager = httpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
            var role = await roleManager.FindByNameAsync("Anonymous");
            var claims = await roleManager.GetClaimsAsync(role);
            var hasClaim = claims.Any(x => x.Type == CustomClaimTypes.Permission && x.Value == roleName);

            return hasClaim;
        }
    }
}