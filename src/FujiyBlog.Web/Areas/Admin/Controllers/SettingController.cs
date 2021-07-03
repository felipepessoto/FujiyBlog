//using FujiyBlog.Core.BlogML;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Infrastructure;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Authorize(nameof(PermissionClaims.AccessAdminSettingsPages))]
    public partial class SettingController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private readonly SettingRepository settingRepository;
        private readonly TelemetryConfiguration configuration;

        public SettingController(FujiyBlogDatabase db, SettingRepository settings, TelemetryConfiguration configuration)
        {
            this.db = db;
            this.settingRepository = settings;
            this.configuration = configuration;
        }

        public ActionResult Index()
        {
            string themeDir = Path.Combine(AppContext.BaseDirectory, "Views", "Themes");
            var fileThemes = Directory.Exists(themeDir) ? new DirectoryInfo(themeDir).GetDirectories().Select(x => new SelectListItem { Text = x.Name }) : Enumerable.Empty<SelectListItem>();

            var themePrefix = "Views_Themes_";
            var themePrefixLength = themePrefix.Length;
            var compiledViewsThemes = CompiledViewsHelper.GetAllViews()
                .Where(x => x.Name.StartsWith(themePrefix))
                .Select(x => x.Name.Substring(themePrefixLength))
                .Select(x => x.Contains('_') ? x.Substring(0, x.IndexOf("_")) : x)
                .Select(x => new SelectListItem { Text = x }).ToList();

            AdminBasicSettings viewModel = new AdminBasicSettings
            {
                BlogName = settingRepository.BlogName,
                BlogDescription = settingRepository.BlogDescription,
                Theme = settingRepository.Theme,
                Themes = fileThemes.Concat(compiledViewsThemes),
                PostsPerPage = settingRepository.PostsPerPage,
                TimeZoneId = settingRepository.TimeZone.Id,
                TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id }),
                Language = settingRepository.Culture,
                ApplicationInsightsInstrumentationKey = settingRepository.ApplicationInsightsInstrumentationKey,
                CustomCode = settingRepository.CustomCode,
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

            settingRepository.BlogName = settings.BlogName;
            settingRepository.BlogDescription = settings.BlogDescription;
            settingRepository.Theme = settings.Theme;
            settingRepository.PostsPerPage = settings.PostsPerPage;
            settingRepository.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZoneId);
            settingRepository.Culture = settings.Language;
            settingRepository.ApplicationInsightsInstrumentationKey = settings.ApplicationInsightsInstrumentationKey;
            settingRepository.CustomCode = settings.CustomCode;

            configuration.DisableTelemetry = string.IsNullOrWhiteSpace(settingRepository.ApplicationInsightsInstrumentationKey);

            if (configuration.DisableTelemetry == false)
            {
                configuration.InstrumentationKey = settingRepository.ApplicationInsightsInstrumentationKey;
            }

            SetSuccessMessage("Settings saved");

            return RedirectToAction("Index");
        }

        public virtual ActionResult Email()
        {
            AdminEmailSettings viewModel = new AdminEmailSettings
            {
                EmailTo = settingRepository.EmailTo,
                EmailSubjectPrefix = settingRepository.EmailSubjectPrefix,
                SmtpAddress = settingRepository.SmtpAddress,
                SmtpPort = settingRepository.SmtpPort,
                SmtpUserName = settingRepository.SmtpUserName,
                SmtpPassword = settingRepository.SmtpPassword,
                SmtpSsl = settingRepository.SmtpSsl,
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

            settingRepository.EmailTo = settings.EmailTo;
            settingRepository.EmailSubjectPrefix = settings.EmailSubjectPrefix;
            settingRepository.SmtpAddress = settings.SmtpAddress;
            settingRepository.SmtpPort = settings.SmtpPort;
            settingRepository.SmtpUserName = settings.SmtpUserName;
            settingRepository.SmtpPassword = settings.SmtpPassword;
            settingRepository.SmtpSsl = settings.SmtpSsl;

            SetSuccessMessage("Settings saved");

            return RedirectToAction("Email");
        }

        public virtual ActionResult Comments()
        {
            AdminCommentsSettings viewModel = new AdminCommentsSettings
            {
                EnableComments = settingRepository.EnableComments,
                ModerateComments = settingRepository.ModerateComments,
                EnableNestedComments = settingRepository.EnableNestedComments,
                CloseCommentsAfterDays = settingRepository.CloseCommentsAfterDays,
                CommentsPerPage = settingRepository.CommentsPerPage,
                CommentsAvatar = settingRepository.CommentsAvatar ?? string.Empty,
                ReCaptchaEnabled = settingRepository.ReCaptchaEnabled,
                ReCaptchaPrivateKey = settingRepository.ReCaptchaPrivateKey,
                ReCaptchaPublicKey = settingRepository.ReCaptchaPublicKey,
                NotifyNewComments = settingRepository.NotifyNewComments,
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

            settingRepository.EnableComments = settings.EnableComments;
            settingRepository.ModerateComments = settings.ModerateComments;
            settingRepository.EnableNestedComments = settings.EnableNestedComments;
            settingRepository.CloseCommentsAfterDays = settings.CloseCommentsAfterDays;
            settingRepository.CommentsPerPage = settings.CommentsPerPage;
            settingRepository.CommentsAvatar = settings.CommentsAvatar;
            settingRepository.ReCaptchaEnabled = settings.ReCaptchaEnabled;
            settingRepository.ReCaptchaPrivateKey = settings.ReCaptchaPrivateKey;
            settingRepository.ReCaptchaPublicKey = settings.ReCaptchaPublicKey;
            settingRepository.NotifyNewComments = settings.NotifyNewComments;

            SetSuccessMessage("Settings saved");

            return RedirectToAction("Comments");
        }

        //TODO
        //public virtual ActionResult Import()
        //{
        //    return View();
        //}

        //[HttpPost, ActionName("Import")]
        //public virtual ActionResult ImportPost()
        //{
        //    var file = Request.Files[0];

        //    if (file == null || file.ContentLength == 0)
        //    {
        //        return Content("Select a file");
        //    }

        //    BlogMLImporter importer = new BlogMLImporter(new BlogMLRepository(db));

        //    importer.Import(file.InputStream);
        //    db.SaveChanges();
            
        //    return RedirectToAction("ImportSuccessful");
        //}

        public virtual ActionResult ImportSuccessful()
        {
            return View();
        }

        public virtual ActionResult SocialNetworks()
        {
            AdminSettingsSocialNetworks viewModel = new AdminSettingsSocialNetworks
            {
                EnableFacebookLikePosts = settingRepository.EnableFacebookLikePosts,
                EnableGooglePlusOnePosts = settingRepository.EnableGooglePlusOnePosts,
                EnableTwitterSharePosts = settingRepository.EnableTwitterSharePosts,
                FacebookAdminIds = settingRepository.FacebookAdminIds,
                FacebookAppId = settingRepository.FacebookAppId,
                OpenGraphImageUrl = settingRepository.OpenGraphImageUrl,
                TwitterBlogAccount = settingRepository.TwitterBlogAccount,
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

            settingRepository.EnableFacebookLikePosts = settings.EnableFacebookLikePosts;
            settingRepository.EnableGooglePlusOnePosts = settings.EnableGooglePlusOnePosts;
            settingRepository.EnableTwitterSharePosts = settings.EnableTwitterSharePosts;
            settingRepository.FacebookAdminIds = settings.FacebookAdminIds;
            settingRepository.FacebookAppId = settings.FacebookAppId;
            settingRepository.OpenGraphImageUrl = settings.OpenGraphImageUrl;
            settingRepository.TwitterBlogAccount = settings.TwitterBlogAccount;

            SetSuccessMessage("Settings saved");

            return RedirectToAction("SocialNetworks");
        }

        public virtual ActionResult Feed()
        {
            AdminFeedSettings viewModel = new AdminFeedSettings
            {
                AlternateFeedUrl = settingRepository.AlternateFeedUrl,
                ItemsShownInFeed = settingRepository.ItemsShownInFeed,
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

            settingRepository.AlternateFeedUrl = settings.AlternateFeedUrl;
            settingRepository.ItemsShownInFeed = settings.ItemsShownInFeed;

            SetSuccessMessage("Settings saved");

            return RedirectToAction("Feed");
        }

        public ActionResult Storage()
        {
            var fileServiceType = typeof(IFileUploadService);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => fileServiceType.IsAssignableFrom(p) && fileServiceType != p)
                .Select(x => x.Name);

            AdminStorageSettings viewModel = new AdminStorageSettings
            {
                FileUploadServices = types.Select(x => new SelectListItem { Text = x }).ToList(),
                FileUploadService = settingRepository.FileUploadService,
                AzureStorageAccountName = settingRepository.AzureStorageAccountName,
                AzureStorageAccountKey = settingRepository.AzureStorageAccountKey,
                AzureStorageUploadContainerName = settingRepository.AzureStorageUploadContainerName,
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Storage(AdminStorageSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            settingRepository.FileUploadService = settings.FileUploadService;
            settingRepository.AzureStorageAccountName = settings.AzureStorageAccountName;
            settingRepository.AzureStorageAccountKey = settings.AzureStorageAccountKey;
            settingRepository.AzureStorageUploadContainerName = settings.AzureStorageUploadContainerName;

            SetSuccessMessage("Settings saved");

            return RedirectToAction(nameof(Storage));
        }

        public ActionResult Cache()
        {
            return View();
        }

        public ActionResult ClearCache()
        {
            db.UpdateLastDbChange();

            SetSuccessMessage("Cache cleared");

            return RedirectToAction("Cache");
        }

        //TODO
        //public ActionResult Logs()
        //{
        //    var targets = LogManager.GetCurrentClassLogger().Factory.Configuration.ConfiguredNamedTargets;
        //    DirectoryInfo logDirectory = new FileInfo(((FileTarget)((NLog.Targets.Wrappers.AsyncTargetWrapper)targets[0]).WrappedTarget).FileName.Render(new LogEventInfo())).Directory;
        //    return View(logDirectory);
        //}

        //public ActionResult LogView(string file)
        //{
        //    var targets = LogManager.GetCurrentClassLogger().Factory.Configuration.ConfiguredNamedTargets;
        //    DirectoryInfo logDirectory = new FileInfo(((FileTarget)((NLog.Targets.Wrappers.AsyncTargetWrapper)targets[0]).WrappedTarget).FileName.Render(new LogEventInfo())).Directory;
        //    FileInfo fileInfo = logDirectory.GetFiles(file).Single();

        //    using (StreamReader reader = fileInfo.OpenText())
        //    {
        //        string content = reader.ReadToEnd();
        //        return Content("<pre>" + Server.HtmlEncode(content) + "</pre>");
        //    }
        //}

        //public ActionResult LogDelete(string file)
        //{
        //    var targets = LogManager.GetCurrentClassLogger().Factory.Configuration.ConfiguredNamedTargets;
        //    DirectoryInfo logDirectory = new FileInfo(((FileTarget)((NLog.Targets.Wrappers.AsyncTargetWrapper)targets[0]).WrappedTarget).FileName.Render(new LogEventInfo())).Directory;
        //    FileInfo fileInfo = logDirectory.GetFiles(file).Single();
        //    fileInfo.Delete();

        //    SetSuccessMessage("Log file " + file + " deleted");

        //    return RedirectToAction(MVC.Admin.Setting.Logs());
        //}

        public void SetSuccessMessage(string str)
        {
            Response.Cookies.Append("NotifyBar", str);
        }
    }
}
