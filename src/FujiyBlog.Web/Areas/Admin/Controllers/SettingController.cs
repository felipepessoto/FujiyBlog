using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.BlogML;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Areas.Admin.Models;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Models;
using System.Globalization;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [AuthorizeRole(Role.AccessAdminSettingsPages)]
    public partial class SettingController : AdminController
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
                                                   Themes = new DirectoryInfo(Server.MapPath("~/Views/Themes/")).GetDirectories().Select(x => new SelectListItem { Text = x.Name }),
                                                   PostsPerPage = Settings.SettingRepository.PostsPerPage,
                                                   TimeZoneId = Settings.SettingRepository.TimeZone.Id,
                                                   TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id }),
                                                   Language = Settings.SettingRepository.Culture,
                                                   Languages = new List<SelectListItem> { new SelectListItem { Text = "Auto", Value = "Auto" }, new SelectListItem { Text = "English", Value = "en" } },
                                                   CustomCode = Settings.SettingRepository.CustomCode,
                                               };

            foreach (FileInfo file in new DirectoryInfo(Server.MapPath("~/App_GlobalResources/")).GetFiles("Labels.*.resx"))
            {
                var culture = CultureInfo.CreateSpecificCulture(file.Name.Substring(7, file.Name.Length - 12));
                viewModel.Languages.Add(new SelectListItem {Text = culture.DisplayName, Value = culture.Name});
            }

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
            Settings.SettingRepository.Culture = settings.Language;
            Settings.SettingRepository.CustomCode = settings.CustomCode;

            return RedirectToAction(MVC.Admin.Setting.Index()).SetSuccessMessage("Settings saved");
        }

        public virtual ActionResult Email()
        {
            AdminEmailSettings viewModel = new AdminEmailSettings
            {
                EmailTo = Settings.SettingRepository.EmailTo,
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

            Settings.SettingRepository.EmailTo = settings.EmailTo;
            Settings.SettingRepository.EmailSubjectPrefix = settings.EmailSubjectPrefix;
            Settings.SettingRepository.SmtpAddress = settings.SmtpAddress;
            Settings.SettingRepository.SmtpPort = settings.SmtpPort;
            Settings.SettingRepository.SmtpUserName = settings.SmtpUserName;
            Settings.SettingRepository.SmtpPassword = settings.SmtpPassword;
            Settings.SettingRepository.SmtpSsl = settings.SmtpSsl;

            return RedirectToAction(MVC.Admin.Setting.Email()).SetSuccessMessage("Settings saved");
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
                CommentsAvatar = Settings.SettingRepository.CommentsAvatar ?? string.Empty,
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

            return RedirectToAction(MVC.Admin.Setting.Comments()).SetSuccessMessage("Settings saved");
        }

        public virtual ActionResult Import()
        {
            return View();
        }

        [HttpPost, ActionName("Import")]
        public virtual ActionResult ImportPost()
        {
            var file = Request.Files[0];

            if (file == null || file.ContentLength == 0)
            {
                return Content("Select a file");
            }

            BlogMLImporter importer = new BlogMLImporter(new BlogMLRepository(db));

            importer.Import(file.InputStream);
            db.SaveChanges();
            return RedirectToAction(MVC.Admin.Setting.ImportSuccessful());
        }

        public virtual ActionResult ImportSuccessful()
        {
            return View();
        }

        public virtual ActionResult SocialNetworks()
        {
            AdminSettingsSocialNetworks viewModel = new AdminSettingsSocialNetworks
            {
                EnableFacebookLikePosts = Settings.SettingRepository.EnableFacebookLikePosts,
                EnableGooglePlusOnePosts = Settings.SettingRepository.EnableGooglePlusOnePosts,
                EnableTwitterSharePosts = Settings.SettingRepository.EnableTwitterSharePosts,
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult SocialNetworks(AdminSettingsSocialNetworks settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Settings.SettingRepository.EnableFacebookLikePosts = settings.EnableFacebookLikePosts;
            Settings.SettingRepository.EnableGooglePlusOnePosts = settings.EnableGooglePlusOnePosts;
            Settings.SettingRepository.EnableTwitterSharePosts = settings.EnableTwitterSharePosts;

            return RedirectToAction(MVC.Admin.Setting.SocialNetworks()).SetSuccessMessage("Settings saved");
        }

        public virtual ActionResult Feed()
        {
            AdminFeedSettings viewModel = new AdminFeedSettings
            {
                AlternateFeedUrl = Settings.SettingRepository.AlternateFeedUrl,
                ItemsShownInFeed = Settings.SettingRepository.ItemsShownInFeed,
                DefaultFeedOutput = Settings.SettingRepository.DefaultFeedOutput,
                DefaultFeedOutputs = new List<SelectListItem> { new SelectListItem { Text = "RSS", Value = "RSS" }, new SelectListItem { Text = "Atom", Value = "Atom" } },
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Feed(AdminFeedSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Settings.SettingRepository.AlternateFeedUrl = settings.AlternateFeedUrl;
            Settings.SettingRepository.ItemsShownInFeed = settings.ItemsShownInFeed;
            Settings.SettingRepository.DefaultFeedOutput = settings.DefaultFeedOutput;

            return RedirectToAction(MVC.Admin.Setting.Feed()).SetSuccessMessage("Settings saved");
        }
    }
}
