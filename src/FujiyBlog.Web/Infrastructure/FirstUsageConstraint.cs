using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace FujiyBlog.Web.Infrastructure
{
    public class FirstUsageConstraint : IRouteConstraint
    {
        private static bool hasUsers;

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (string.Equals(values["controller"] as string, "Account", StringComparison.OrdinalIgnoreCase) && string.Equals(values["action"] as string, "Register", StringComparison.OrdinalIgnoreCase))
            {
                if (hasUsers == false)
                {
                    var db = httpContext.RequestServices.GetRequiredService<FujiyBlogDatabase>();
                    var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = httpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                    FujiyBlogDatabaseInitializer.SeedDatabase(db, roleManager).Wait();
                    hasUsers = userManager.Users.Any();              
                }

                return hasUsers == false;
            }

            return true;
        }
    }
}