using System.Web.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public partial class JavaScriptController : AbstractController
    {
        [OutputCache(Duration = 60 * 60 * 24 * 7, VaryByParam = "")]
        public virtual ActionResult FujiyBlogUrls()
        {
            return View();
        }
    }
}
