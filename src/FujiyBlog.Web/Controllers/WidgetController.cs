using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Controllers
{
    public partial class WidgetController : AbstractController
    {
        private readonly FujiyBlogDatabase db;
        private readonly WidgetSettingRepository widgetSettingRepository;
        private static string[] widgets;

        public WidgetController(FujiyBlogDatabase db, WidgetSettingRepository widgetSettingRepository)
        {
            this.db = db;
            this.widgetSettingRepository = widgetSettingRepository;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (widgets == null)
            {
                var diretorio = new DirectoryInfo(Server.MapPath("~/Views/Widget/"));
                widgets = (from file in diretorio.GetFiles()
                           let fileWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName)
                           where fileWithoutExtension != "Index" && fileWithoutExtension != "Widget" && !fileWithoutExtension.EndsWith("Edit")
                           select fileWithoutExtension).ToArray();
            }
        }

        [ChildActionOnly]
        public virtual ActionResult Index(string zoneName)
        {
            WidgetIndex viewModel = new WidgetIndex
                                        {
                                            WidgetSettings = CacheHelper.FromCacheOrExecute(() => widgetSettingRepository.GetWidgetSettings(zoneName)),
                                            AvailableWidgets = widgets,
                                            ZoneName = zoneName
                                        };

            return View(viewModel);
        }

        [HttpPost, AuthorizeRole(Role.ManageWidgets)]
        public virtual ActionResult Add(string zoneName, string widgetName)
        {
            if (string.IsNullOrEmpty(widgetName))
            {
                return Json(false);
            }

            WidgetSetting widgetSetting = new WidgetSetting
                                              {
                                                  Name = widgetName,
                                                  WidgetZone = zoneName,
                                                  Position = int.MaxValue
                                              };

            widgetSettingRepository.Add(widgetSetting);
            db.SaveChanges();
            return View(widgetSetting.Name, widgetSetting);
        }

        [HttpPost, AuthorizeRole(Role.ManageWidgets)]
        public virtual ActionResult Remove(int settingsId)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(settingsId);
            widgetSettingRepository.Remove(setting);
            db.SaveChanges();
            return Json(true);
        }

        [AuthorizeRole(Role.ManageWidgets)]
        public virtual ActionResult Edit(int widgetSettingId)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);

            return View(setting.Name + "Edit", setting);
        }

        [AuthorizeRole(Role.ManageWidgets), HttpPost, ValidateInput(false)]
        public virtual ActionResult Edit(int widgetSettingId, string settings)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);

            setting.Settings = settings;

            db.SaveChanges();

            return View(setting.Name, setting);
        }

        [AuthorizeRole(Role.ManageWidgets), HttpPost]
        public virtual ActionResult Sort(string widgetsOrder)
        {
            int position = 1;
            foreach (int widgetSettingId in widgetsOrder.Split(',').Select(x=> int.Parse(x.Substring(6))))
            {
                WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);
                setting.Position = position++;
            }
            db.SaveChanges();
            return null;
        }
    }
}
