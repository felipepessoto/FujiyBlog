using System.Web.Mvc;

namespace FujiyBlog.Web.Extensions
{
    public static class WebPageBaseExtensions
    {
        public static string FindLayoutPath(this WebViewPage webViewPage, string layoutName)
        {
            return ((RazorView)ViewEngines.Engines.FindView(webViewPage.ViewContext.Controller.ControllerContext, layoutName, "").View).ViewPath;
        }
    }
}