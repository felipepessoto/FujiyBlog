using System;
using System.Linq;
using System.Web.Caching;
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
            const string cacheKey = "FujiyBlog.Web.Controllers.JavaScriptController.FujiyBlogBundle";

            string bundleContent = HttpContext.Cache[cacheKey] as string;

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

                files = files.Select(Server.MapPath).ToArray();
                bundleContent = string.Join(Environment.NewLine, files.Select(System.IO.File.ReadAllText));

                HttpContext.Cache.Add(cacheKey, bundleContent, new CacheDependency(files), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }

            ViewBag.Bundle = bundleContent;

            return View();
        }
    }
}
