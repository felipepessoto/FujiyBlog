using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public partial class JavaScriptController : Controller
    {
        public virtual ActionResult FujiyBlog()
        {
            return View(MVC.Shared.Views.FujiyBlogJs);
        }
    }
}
