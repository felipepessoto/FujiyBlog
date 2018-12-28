using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FujiyBlog.Web.ViewComponents
{
    public partial class WidgetViewComponent : ViewComponent
    {
        private readonly FujiyBlogDatabase db;
        private readonly WidgetSettingRepository widgetSettingRepository;
        private static string[] widgets;

        public WidgetViewComponent(FujiyBlogDatabase db, WidgetSettingRepository widgetSettingRepository)
        {
            this.db = db;
            this.widgetSettingRepository = widgetSettingRepository;

            if (widgets == null || widgets.Length == 0)
            {
                var widgetsViews = CompiledViewsHelper.GetViewsTypes<WidgetSetting>();

                widgets = (from widgetView in widgetsViews
                           let nameWithoutPrefix = widgetView.Name.Substring("Views_Shared_Components_Widget_".Length)
                           where nameWithoutPrefix != "Index" && nameWithoutPrefix != "Widget" && !nameWithoutPrefix.EndsWith("Edit")
                           select nameWithoutPrefix).ToArray();
            }
        }

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
        //    if (widgets == null)
        //    {
        //        var diretorio = new DirectoryInfo(Server.MapPath("~/Views/Widget/"));
        //        widgets = (from file in diretorio.GetFiles()
        //                   let fileWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName)
        //                   where fileWithoutExtension != "Index" && fileWithoutExtension != "Widget" && !fileWithoutExtension.EndsWith("Edit")
        //                   select fileWithoutExtension).ToArray();
        //    }
        //}

        public Task<IViewComponentResult> InvokeAsync(string zoneName)
        {
            WidgetIndex viewModel = new WidgetIndex
            {
                WidgetSettings = CacheHelper.FromCacheOrExecute(db, () => widgetSettingRepository.GetWidgetSettings(zoneName)),
                AvailableWidgets = widgets,
                ZoneName = zoneName
            };

            return Task.FromResult<IViewComponentResult>(View("Index", viewModel));
        }
    }
}
