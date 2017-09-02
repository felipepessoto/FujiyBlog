using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
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
                Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                var precompiledViews = allAssemblies.Where(x => x.GetName().Name == "FujiyBlog.Web.PrecompiledViews").ToList();

                IEnumerable<Assembly> assemblies =
                    precompiledViews.Any()
                    ? precompiledViews
                    : allAssemblies.Where(x => x.GetName().Name.StartsWith("Microsoft") == false && x.GetName().Name.StartsWith("System") == false && x.GetName().Name.StartsWith("netstandard") == false && x.GetName().Name.StartsWith("mscorlib") == false).ToList();

                var widgetsViews = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(x => x.Name.StartsWith("_Views_Shared_Components_Widget_")).ToList();
                widgetsViews = widgetsViews.Where(type => type.IsSubclassOf(typeof(RazorPage<WidgetSetting>))).ToList();

                widgets = (from widgetView in widgetsViews
                           let nameWithoutPrefix = widgetView.Name.Substring("_Views_Shared_Components_Widget_".Length)
                           let nameWithoutSuffix = nameWithoutPrefix.Substring(0, nameWithoutPrefix.Length - 7)
                           where nameWithoutSuffix != "Index" && nameWithoutSuffix != "Widget" && !nameWithoutSuffix.EndsWith("Edit")
                           select nameWithoutSuffix).ToArray();
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
