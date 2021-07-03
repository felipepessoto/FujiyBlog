using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FujiyBlog.Web.ViewComponents
{
    public class WidgetController : Controller
    {
        private const string viewsPath = "~/Views/Shared/Components/Widget/";
        private readonly FujiyBlogDatabase db;
        private readonly WidgetSettingRepository widgetSettingRepository;

        public WidgetController(FujiyBlogDatabase db, WidgetSettingRepository widgetSettingRepository)
        {
            this.db = db;
            this.widgetSettingRepository = widgetSettingRepository;
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

        [HttpPost, Authorize(nameof(PermissionClaims.ManageWidgets))]
        public ActionResult Add(string zoneName, string widgetName)
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
            return View(viewsPath + widgetSetting.Name + ".cshtml", widgetSetting);
        }

        [HttpPost, Authorize(nameof(PermissionClaims.ManageWidgets))]
        public ActionResult Remove(int id)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(id);
            widgetSettingRepository.Remove(setting);
            db.SaveChanges();
            return Json(true);
        }

        [Authorize(nameof(PermissionClaims.ManageWidgets))]
        public ActionResult Edit(int id)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(id);

            return View(viewsPath + setting.Name + "Edit.cshtml", setting);
        }

        [Authorize(nameof(PermissionClaims.ManageWidgets)), HttpPost]
        public ActionResult Edit(int id, string settings)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(id);

            setting.Settings = settings;

            db.SaveChanges();

            return View(viewsPath + setting.Name + ".cshtml", setting);
        }

        [Authorize(nameof(PermissionClaims.ManageWidgets)), HttpPost]
        public virtual ActionResult Sort(string widgetsOrder)
        {
            int position = 1;
            foreach (int widgetSettingId in widgetsOrder.Split(',').Select(x => int.Parse(x.Substring(6))))
            {
                WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);
                setting.Position = position++;
            }
            db.SaveChanges();
            return null;
        }
    }
}
