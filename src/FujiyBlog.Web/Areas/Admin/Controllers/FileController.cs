using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class FileController : AdminController
    {
        [HttpPost]
        public virtual ActionResult Upload(HttpPostedFileBase uploadFile, string folderName)
        {
            if (uploadFile == null || uploadFile.ContentLength <= 0)
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

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(HttpContext.Server.MapPath(path));
                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }

                string possiblePath = VirtualPathUtility.Combine(path, uploadFile.FileName);

                int fileCount = 2;
                while (System.IO.File.Exists(HttpContext.Server.MapPath(possiblePath)))
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uploadFile.FileName) + fileCount;
                    string extension = Path.GetExtension(uploadFile.FileName);

                    possiblePath = VirtualPathUtility.Combine(path, fileNameWithoutExtension + extension);

                    fileCount++;
                }

                path = possiblePath;

                uploadFile.SaveAs(HttpContext.Server.MapPath(path));
            }
            catch (IOException)
            {
                return Json(new {errorMessage = "Can´t upload file. Check permissions"});
            }
            return Json(new {url = Url.Content(path)});
        }
    }
}

