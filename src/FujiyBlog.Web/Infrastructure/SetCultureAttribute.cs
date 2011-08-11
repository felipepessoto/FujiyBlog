using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Infrastructure
{
    public class SetCultureAttribute : ActionFilterAttribute 
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                var culture = Settings.SettingRepository.Culture;
                if (!string.IsNullOrWhiteSpace(culture) && !culture.Equals("Auto", StringComparison.InvariantCultureIgnoreCase))
                {
                    CultureInfo selectedCulture = CultureInfo.CreateSpecificCulture(culture);
                    Thread.CurrentThread.CurrentUICulture = selectedCulture;
                    Thread.CurrentThread.CurrentCulture = selectedCulture;
                }
            }
        }
    }
}