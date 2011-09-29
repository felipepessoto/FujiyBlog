using System.Web;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;

namespace FujiyBlog.Core.Services
{
    public class RolesService
    {
        public static bool UserHasPermission(Permission permission)
        {
            return HttpContext.Current.User.IsInRole(permission);

        }
    }
}
