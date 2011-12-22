using System;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Web.Infrastructure;

namespace FujiyBlog.Web.Controllers
{
    public partial class JavaScriptController : AbstractController
    {
        private static string bundleContent;

        [CompressFilter]
        [OutputCache(Duration = 60 * 60 * 24 * 7, VaryByHeader = "Accept-Encoding", VaryByParam = "")]
        public virtual ActionResult FujiyBlogBundle()
        {
            if (bundleContent == null)
            {
                string[] files = new[]
                                 {
                                     "~/Scripts/jquery-1.6.4.min.js",
                                     "~/Scripts/jquery-ui-1.8.16.min.js",
                                     "~/Scripts/jquery.validate.min.js",
                                     "~/Scripts/jquery.validate.unobtrusive.min.js",
                                     "~/Scripts/jquery.unobtrusive-ajax.min.js",
                                     "~/Scripts/jquery-ui-timepicker-addon.js",
                                     "~/Scripts/fujiyblog.js",
                                     "~/Scripts/json2.js",
                                 };

                bundleContent = string.Join(Environment.NewLine, files.Select(x => System.IO.File.ReadAllText(Server.MapPath(x))));
            }

            ViewBag.Bundle = bundleContent;

            return View();
        }
    }
}
