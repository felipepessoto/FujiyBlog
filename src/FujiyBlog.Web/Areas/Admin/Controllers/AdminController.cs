using System.Web.Mvc;
using FujiyBlog.Web.Controllers;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Authorize]
    public abstract partial class AdminController : AbstractController
    {
    }
}
