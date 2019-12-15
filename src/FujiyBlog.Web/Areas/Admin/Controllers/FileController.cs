using FujiyBlog.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class FileController : AdminController
    {
        private readonly IFileUploadService fileUploadService;

        public FileController(IFileUploadService fileUploadService)
        {
            this.fileUploadService = fileUploadService;
        }

        [HttpPost]
        public async Task<ActionResult> Upload(IFormFile[] uploadedFiles, string folderName)
        {
            if (uploadedFiles == null || uploadedFiles.Contains(null) || uploadedFiles.Length == 0)
            {
                return Json(new { errorMessage = "The Uploaded file is empty" });
            }

            if (folderName.Any(x=> char.IsLetterOrDigit(x) == false && x != '-'))
            {
                return Json(new { errorMessage = "Invalid Folder" });
            }

            string folderRelativePath;

            if (!string.IsNullOrEmpty(folderName))
            {
                folderRelativePath = folderName;
            }
            else
            {
                DateTime now = DateTime.Now;
                folderRelativePath = Path.Combine(now.Year.ToString(), now.Month.ToString());
            }

            List<string> paths = new List<string>(uploadedFiles.Length);

            foreach (var uploadedFile in uploadedFiles)
            {
                var fileName = await fileUploadService.UploadFile(uploadedFile.OpenReadStream(), Path.Combine(folderRelativePath, Path.GetFileName(uploadedFile.FileName)));
                paths.Add(fileName);
            }

            return Json(new { urls = paths.Select(x => Url.Content("~/Upload/" + x.Replace("\\", "/"))) });
        }
    }
}

