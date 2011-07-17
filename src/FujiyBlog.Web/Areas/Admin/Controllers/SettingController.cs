using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.BlogML;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class SettingController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public SettingController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ActionResult Index()
        {
            AdminBasicSettings viewModel = new AdminBasicSettings
                                               {
                                                   BlogName = Settings.SettingRepository.BlogName,
                                                   BlogDescription = Settings.SettingRepository.BlogDescription,
                                                   Theme = Settings.SettingRepository.Theme,
                                                   PostsPerPage = Settings.SettingRepository.PostsPerPage,
                                                   TimeZoneId = Settings.SettingRepository.TimeZone.Id,
                                                   TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id }),
                                                   Themes = new DirectoryInfo(Server.MapPath("~/Views/Themes/")).GetDirectories().Select(x => new SelectListItem { Text = x.Name }),
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
            Settings.SettingRepository.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZoneId);

            return RedirectToAction(MVC.Admin.Setting.Index());
        }

        public virtual ActionResult Email()
        {
            AdminEmailSettings viewModel = new AdminEmailSettings
            {
                EmailFrom = Settings.SettingRepository.EmailFrom,
                EmailSubjectPrefix = Settings.SettingRepository.EmailSubjectPrefix,
                SmtpAddress = Settings.SettingRepository.SmtpAddress,
                SmtpPort = Settings.SettingRepository.SmtpPort,
                SmtpUserName = Settings.SettingRepository.SmtpUserName,
                SmtpPassword = Settings.SettingRepository.SmtpPassword,
                SmtpSsl = Settings.SettingRepository.SmtpSsl,
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Email(AdminEmailSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Settings.SettingRepository.EmailFrom = settings.EmailFrom;
            Settings.SettingRepository.EmailSubjectPrefix = settings.EmailSubjectPrefix;
            Settings.SettingRepository.SmtpAddress = settings.SmtpAddress;
            Settings.SettingRepository.SmtpPort = settings.SmtpPort;
            Settings.SettingRepository.SmtpUserName = settings.SmtpUserName;
            Settings.SettingRepository.SmtpPassword = settings.SmtpPassword;
            Settings.SettingRepository.SmtpSsl = settings.SmtpSsl;

            return RedirectToAction(MVC.Admin.Setting.Email());
        }

        public virtual ActionResult Comments()
        {
            AdminCommentsSettings viewModel = new AdminCommentsSettings
            {
                EnableComments = Settings.SettingRepository.EnableComments,
                ModerateComments = Settings.SettingRepository.ModerateComments,
                EnableNestedComments = Settings.SettingRepository.EnableNestedComments,
                CloseCommentsAfterDays = Settings.SettingRepository.CloseCommentsAfterDays,
                CommentsPerPage = Settings.SettingRepository.CommentsPerPage,
                CommentsAvatar = Settings.SettingRepository.CommentsAvatar,
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Comments(AdminCommentsSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Settings.SettingRepository.EnableComments = settings.EnableComments;
            Settings.SettingRepository.ModerateComments = settings.ModerateComments;
            Settings.SettingRepository.EnableNestedComments = settings.EnableNestedComments;
            Settings.SettingRepository.CloseCommentsAfterDays = settings.CloseCommentsAfterDays;
            Settings.SettingRepository.CommentsPerPage = settings.CommentsPerPage;
            Settings.SettingRepository.CommentsAvatar = settings.CommentsAvatar;

            return RedirectToAction(MVC.Admin.Setting.Comments());
        }

        public virtual ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult ImportBlogML()
        {
            var file = Request.Files[0];

            if (file == null || file.ContentLength == 0)
            {
                return Content("Select a file");
            }

            BlogMLImporter importer = new BlogMLImporter(new BlogMLRepository(db));

            importer.Import(file.InputStream);
            db.SaveChanges();
            return Content("OK");
        }
    }
}
