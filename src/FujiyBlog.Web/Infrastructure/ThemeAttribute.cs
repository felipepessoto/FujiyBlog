using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Infrastructure
{
    public class ThemeAttribute : ActionFilterAttribute 
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                RazorViewEngine engine = (RazorViewEngine)ViewEngines.Engines.Single();

                string themeName = Settings.SettingRepository.Theme;

                engine.MasterLocationFormats = new[]
                                               {
                                                   "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                   "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                   "~/Views/{1}/{0}.cshtml",
                                                   "~/Views/Shared/{0}.cshtml",
                                               };

                engine.ViewLocationFormats = new[]
                                             {
                                                 "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                 "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                  "~/Views/{1}/{0}.cshtml",
                                                   "~/Views/Shared/{0}.cshtml",
                                             };

                engine.PartialViewLocationFormats = new[]
                                                    {
                                                        "~/Views/Themes/" + themeName + "/{1}/{0}.cshtml",
                                                        "~/Views/Themes/" + themeName + "/Shared/{0}.cshtml",
                                                        "~/Views/{1}/{0}.cshtml",
                                                        "~/Views/Shared/{0}.cshtml",
                                                    };
            }
        }
    }
}