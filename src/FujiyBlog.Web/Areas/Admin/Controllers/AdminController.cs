using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(nameof(PermissionClaims.AccessAdminPages))]
    public abstract class AdminController : Controller
    {
    }
}
