using System.Web.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public partial class JavaScriptController : AbstractController
    {
        [OutputCache(Duration = int.MaxValue)]
        public virtual ActionResult FujiyBlog()
        {
            return View(MVC.Shared.Views.FujiyBlogJs);
        }
    }
}
