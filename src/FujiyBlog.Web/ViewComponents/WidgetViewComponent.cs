using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Linq;
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

            if (widgets == null)
            {
                var diretorio = new DirectoryInfo(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Views", "Shared", "Components", "Widget"));
                widgets = (from file in diretorio.GetFiles()
                           let fileWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName)
                           where fileWithoutExtension != "Index" && fileWithoutExtension != "Widget" && !fileWithoutExtension.EndsWith("Edit")
                           select fileWithoutExtension).ToArray();
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
