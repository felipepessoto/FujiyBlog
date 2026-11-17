using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminEmailSettings
    {
        [Required, EmailAddress]
        [Display(Name = "Email To")]
        public string EmailTo { get; set; } = string.Empty;

        [Display(Name = "Subject Prefix")]
        public string? EmailSubjectPrefix { get; set; }

        [Required]
        [Display(Name = "Smtp Address")]
        public string SmtpAddress { get; set; } = string.Empty;

        [Display(Name = "Smtp Port")]
        [Range(1, 65535)]
        public int SmtpPort { get; set; }

        [Display(Name = "Smtp UserName")]
        public string? SmtpUserName { get; set; }

        [Display(Name = "Smtp Password")]
        public string? SmtpPassword { get; set; }

        [Display(Name = "Enable SSL")]
        public bool SmtpSsl { get; set; }
    }
}