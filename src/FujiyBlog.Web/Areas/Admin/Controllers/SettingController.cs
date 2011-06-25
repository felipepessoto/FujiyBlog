using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class SettingController : Controller
    {
        public virtual ActionResult Index()
        {
            AdminBasicSettings viewModel = new AdminBasicSettings
                                               {
                                                   BlogName = Settings.SettingRepository.BlogName,
                                                   BlogDescription = Settings.SettingRepository.BlogDescription,
                                                   Theme = Settings.SettingRepository.Theme,
                                                   PostsPerPage = Settings.SettingRepository.PostsPerPage,
                                                   TimeZoneId = Settings.SettingRepository.TimeZoneId.Id
                                               };

            return View(viewModel);
        }

    }
}
