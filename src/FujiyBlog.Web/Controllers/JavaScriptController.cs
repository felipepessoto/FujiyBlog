using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System;
using System.Web.UI;

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

        public class CompressFilter : ActionFilterAttribute
        {
            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {
                if (filterContext.Exception != null)
                {
                    return;
                }

                HttpRequestBase request = filterContext.HttpContext.Request;

                string acceptEncoding = request.Headers["Accept-Encoding"];

                if (string.IsNullOrEmpty(acceptEncoding)) return;

                acceptEncoding = acceptEncoding.ToUpperInvariant();

                HttpResponseBase response = filterContext.HttpContext.Response;

                if (acceptEncoding.Contains("GZIP"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
        }
    }
}
