using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminStorageSettings
    {
        [BindNever, ValidateNever]
        public IEnumerable<SelectListItem> FileUploadServices { get; set; } = [];

        [Required]
        [Display(Name = "File upload service")]
        public string FileUploadService { get; set; } = string.Empty;

        [Display(Name = "Azure Storage Account Name")]
        public string? AzureStorageAccountName { get; set; }

        [Display(Name = "Azure Storage Account Key")]
        public string? AzureStorageAccountKey { get; set; }

        [Display(Name = "Azure Storage Upload Container Name")]
        public string? AzureStorageUploadContainerName { get; set; }
    }
}