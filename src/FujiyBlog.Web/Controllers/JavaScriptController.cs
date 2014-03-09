using System.Web.Mvc;
using FujiyBlog.Web.Infrastructure;

namespace FujiyBlog.Web.Controllers
{
    public partial class JavaScriptController : AbstractController
    {
        [CompressFilter]
        [OutputCache(Duration = 60 * 60 * 24 * 7, VaryByHeader = "Accept-Encoding", VaryByParam = "")]
        public virtual ActionResult FujiyBlogBundle()
        {
            return View();
        }
    }
}
