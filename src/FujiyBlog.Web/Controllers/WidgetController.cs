﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Web.ViewModels;
using System.IO;

namespace FujiyBlog.Web.Controllers
{
    public partial class WidgetController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWidgetSettingRepository widgetSettingRepository;
        private static string[] widgets;

        public WidgetController(IUnitOfWork unitOfWork, IWidgetSettingRepository widgetSettingRepository)
        {
            this.unitOfWork = unitOfWork;
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

        public virtual ActionResult Index(string zoneName)
        {
            WidgetIndex viewModel = new WidgetIndex
                                        {
                                            WidgetSettings = widgetSettingRepository.GetWidgetSettings(zoneName),
                                            AvailableWidgets = widgets,
                                            ZoneName = zoneName
                                        };

            return View(viewModel);
        }

        [HttpPost, Authorize]
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
            unitOfWork.SaveChanges();
            return View(widgetSetting.Name, widgetSetting);
        }

        [HttpPost, Authorize]
        public virtual ActionResult Remove(int settingsId)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(settingsId);
            widgetSettingRepository.Remove(setting);
            unitOfWork.SaveChanges();
            return Json(true);
        }

        [Authorize]
        public virtual ActionResult Edit(int widgetSettingId)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);

            return View(setting.Name + "Edit", setting);
        }

        [Authorize, HttpPost, ValidateInput(false)]
        public virtual ActionResult Edit(int widgetSettingId, string settings)
        {
            WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);

            setting.Settings = settings;

            unitOfWork.SaveChanges();

            return View(setting.Name, setting);
        }

        [Authorize, HttpPost]
        public virtual ActionResult Sort(string widgetsOrder)
        {
            int position = 1;
            foreach (int widgetSettingId in widgetsOrder.Split(',').Select(x=> int.Parse(x.Substring(6))))
            {
                WidgetSetting setting = widgetSettingRepository.GetWidgetSetting(widgetSettingId);
                setting.Position = position++;
            }
            unitOfWork.SaveChanges();
            return null;
        }
    }
}
