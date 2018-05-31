using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Extensions
{
    public static class PrincipalExtensions
    {
        const string GetAnonymousRolesCacheKey = "FujiyBlog.Core.Extensions.PrincipalExtensions.GetAnonymousRoles";

        public static bool UserHasClaimPermission(this HttpContext context, PermissionClaims permission)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.HasClaim(CustomClaimTypes.Permission, permission.ToString());
            }

            return GetAnonymousRoles(context, permission).GetAwaiter().GetResult(); //TODO async
        }


        private static Task<bool> GetAnonymousRoles(HttpContext httpContext, PermissionClaims permission)
        {
            string roleName = permission.ToString();
            
            var claims = httpContext.Items[GetAnonymousRolesCacheKey] as IList<Claim>;

            if (claims != null)
            {
                var hasClaimFromCache = claims.Any(x => x.Type == CustomClaimTypes.Permission && x.Value == roleName);
                return Task.FromResult(hasClaimFromCache);
            }

            return GetAnonymousRolesFromDb(httpContext, permission);
        }

        private static async Task<bool> GetAnonymousRolesFromDb(HttpContext httpContext, PermissionClaims permission)
        {
            string roleName = permission.ToString();

            var roleManager = httpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
            var role = await roleManager.FindByNameAsync("Anonymous");

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            httpContext.Items[GetAnonymousRolesCacheKey] = claims;

            var hasClaim = claims.Any(x => x.Type == CustomClaimTypes.Permission && x.Value == roleName);
            return hasClaim;
        }
    }
}