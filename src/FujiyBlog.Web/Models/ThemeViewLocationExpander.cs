using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace FujiyBlog.Web.Models
{
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "theme";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var theme = context.ActionContext.HttpContext.RequestServices.GetRequiredService<SettingRepository>().Theme;
            context.Values[THEME_KEY] = theme;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == null)
            {
                string theme = null;
                if (context.Values.TryGetValue(THEME_KEY, out theme))
                {
                    viewLocations = new[] {
                $"/Views/Themes/{theme}/{{1}}/{{0}}.cshtml",
                $"/Views/Themes/{theme}/Shared/{{0}}.cshtml",
            }
                    .Concat(viewLocations);
                }
            }

            return viewLocations;
        }
    }


}
