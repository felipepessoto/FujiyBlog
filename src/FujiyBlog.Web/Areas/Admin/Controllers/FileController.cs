using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class FileController : AdminController
    {
        private readonly IHostingEnvironment env;

        public FileController(IHostingEnvironment env)
        {
            this.env = env;
        }

        [HttpPost]
        public ActionResult Upload(IFormFile[] uploadedFiles, string folderName)
        {
            if (uploadedFiles == null || uploadedFiles.Contains(null) || uploadedFiles.Length == 0)
            {
                return Json(new {errorMessage = "The Uploaded file is empty"});
            }
            if (!Directory.Exists(Path.Combine(env.WebRootPath, "Upload")))
            {
                return Json(new {errorMessage = "The Upload folder don´t exists"});
            }

            string folderRelativePath;

            if (!string.IsNullOrEmpty(folderName))
            {
                folderRelativePath = Path.Combine("Upload", folderName);
            }
            else
            {
                DateTime now = DateTime.Now;
                folderRelativePath = Path.Combine("Upload", now.Year.ToString(), now.Month.ToString());
            }

            List<string> paths = new List<string>(uploadedFiles.Length);

            try
            {
                var phisicalFolder = Path.Combine(env.WebRootPath, folderRelativePath);
                DirectoryInfo dInfo = new DirectoryInfo(phisicalFolder);
                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }

                foreach (var uploadedFile in uploadedFiles)
                {
                    string possibleRelativeFilePath = Path.Combine(folderRelativePath, Path.GetFileName(uploadedFile.FileName));

                    int fileCount = 2;
                    while (System.IO.File.Exists(Path.Combine(env.WebRootPath, possibleRelativeFilePath)))
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + fileCount;
                        string extension = Path.GetExtension(uploadedFile.FileName);

                        possibleRelativeFilePath = Path.Combine(folderRelativePath, fileNameWithoutExtension + extension);

                        fileCount++;
                    }

                    using (var fileStream = new FileStream(Path.Combine(env.WebRootPath, possibleRelativeFilePath), FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }
                    paths.Add(possibleRelativeFilePath);
                }
            }
            catch (IOException)
            {
                return Json(new {errorMessage = "Can´t upload file. Check permissions"});
            }
            return Json(new { urls = paths.Select(x=> Url.Content("~/" + x.Replace("\\", "/"))) });
        }
    }
}

