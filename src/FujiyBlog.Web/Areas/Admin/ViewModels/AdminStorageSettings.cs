using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminStorageSettings
    {
        public IEnumerable<SelectListItem> FileUploadServices { get; set; }

        [Display(Name = "File upload service")]
        public string FileUploadService { get; set; }

        [Display(Name = "Azure Storage Account Name")]
        public string AzureStorageAccountName { get; set; }

        [Display(Name = "Azure Storage Account Key")]
        public string AzureStorageAccountKey { get; set; }

        [Display(Name = "Azure Storage Upload Container Name")]
        public string AzureStorageUploadContainerName { get; set; }
    }
}