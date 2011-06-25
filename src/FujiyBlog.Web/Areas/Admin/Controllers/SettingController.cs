using System;
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

        [HttpPost]
        public virtual ActionResult Index(AdminBasicSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Settings.SettingRepository.BlogName = settings.BlogName;
            Settings.SettingRepository.BlogDescription = settings.BlogDescription;
            Settings.SettingRepository.Theme = settings.Theme;
            Settings.SettingRepository.PostsPerPage = settings.PostsPerPage;
            Settings.SettingRepository.TimeZoneId = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZoneId);

            return RedirectToAction(MVC.Admin.Setting.Index());
        }
    }
}
