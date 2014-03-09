using System.Web;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.Models
{
    public static class ActionResultExtensions
    {
        public static ActionResult SetSuccessMessage(this ActionResult ar, string str)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("NotifyBar", str));
            return ar;
        }
    }
}