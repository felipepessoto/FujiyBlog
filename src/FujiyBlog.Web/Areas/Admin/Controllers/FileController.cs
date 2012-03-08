using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class FileController : AdminController
    {
        [HttpPost]
        public virtual ActionResult Upload(HttpPostedFileBase[] uploadedFiles, string folderName)
        {
            if (uploadedFiles == null || uploadedFiles.Contains(null) || uploadedFiles.Length == 0)
            {
                return Json(new {errorMessage = "The Uploaded file is empty"});
            }
            if (!Directory.Exists(HttpContext.Server.MapPath("~/Upload")))
            {
                return Json(new {errorMessage = "The Upload folder don´t exists"});
            }

            string path;

            if (!string.IsNullOrEmpty(folderName))
            {
                path = "~/Upload/" + folderName + "/";
            }
            else
            {
                DateTime now = DateTime.Now;
                path = "~/Upload/" + now.Year + "/" + now.Month + "/";
            }

            List<string> paths = new List<string>(uploadedFiles.Length);

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(HttpContext.Server.MapPath(path));
                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }

                foreach (HttpPostedFileBase uploadedFile in uploadedFiles)
                {
                    string possiblePath = VirtualPathUtility.Combine(path, uploadedFile.FileName);

                    int fileCount = 2;
                    while (System.IO.File.Exists(HttpContext.Server.MapPath(possiblePath)))
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + fileCount;
                        string extension = Path.GetExtension(uploadedFile.FileName);

                        possiblePath = VirtualPathUtility.Combine(path, fileNameWithoutExtension + extension);

                        fileCount++;
                    }

                    path = possiblePath;

                    uploadedFile.SaveAs(HttpContext.Server.MapPath(path));
                    paths.Add(path);
                }
            }
            catch (IOException)
            {
                return Json(new {errorMessage = "Can´t upload file. Check permissions"});
            }
            return Json(new {urls = paths.Select(Url.Content) });
        }
    }
}

