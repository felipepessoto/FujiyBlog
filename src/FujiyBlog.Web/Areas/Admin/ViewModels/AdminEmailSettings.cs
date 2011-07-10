using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminEmailSettings
    {
        [Display(Name = "Email From")]
        public string EmailFrom { get; set; }

        [Display(Name = "Subject Prefix")]
        public string EmailSubjectPrefix { get; set; }

        [Display(Name = "SMTP Server")]
        public string SmtpAddress { get; set; }

        [Display(Name = "SMTP Port")]
        public int SmtpPort { get; set; }

        [Display(Name = "Smtp UserName")]
        public string SmtpUserName { get; set; }

        [Display(Name = "Smtp Password")]
        public string SmtpPassword { get; set; }

        [Display(Name = "Enable SSL")]
        public bool SmtpSsl { get; set; }
    }
}