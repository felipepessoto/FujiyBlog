using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Controllers;
using FujiyBlog.Web.Infrastructure;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [AuthorizeRole(Role.AccessAdminPages)]
    public abstract partial class AdminController : AbstractController
    {
    }
}
