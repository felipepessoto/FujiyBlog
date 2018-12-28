using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly SettingRepository settings;

        public UploadController(SettingRepository settings)
        {
            this.settings = settings;
        }

        public IActionResult Details(string filePath)
        {
            return RedirectPermanent($"https://{settings.AzureStorageAccountName}.blob.core.windows.net/{settings.AzureStorageUploadContainerName}/" + filePath);
        }
    }
}