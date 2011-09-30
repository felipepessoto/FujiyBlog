using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Extensions;

namespace FujiyBlog.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public AuthorizePermissionAttribute(params Permission[] permissions)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException("permissions");
            }

            Roles = string.Join(",", permissions.Select(r => r.ToString()));
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;

            var usersSplit = SplitString(Users);
            var rolesSplit = SplitString(Roles).Select(x => (Permission) Enum.Parse(typeof (Permission), x)).ToArray();

            if (usersSplit.Length > 0 && !usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (rolesSplit.Length > 0 && !rolesSplit.Any(user.IsInRole))
            {
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            filterContext.HttpContext.Response.SendToUnauthorized();
        }

        static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}