using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Web.Controllers
{
    public partial class WidgetController : Controller
    {
        private readonly IWidgetSettingRepository widgetSettingRepository;

        public WidgetController(IWidgetSettingRepository widgetSettingRepository)
        {
            this.widgetSettingRepository = widgetSettingRepository;
        }

        public virtual ActionResult Index(string zoneName)
        {
            IEnumerable<WidgetSetting> settings = widgetSettingRepository.GetWidgetSettings(zoneName);

            return View(settings);
        }
    }
}
