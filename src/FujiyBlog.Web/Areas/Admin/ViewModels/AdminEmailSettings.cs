using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminEmailSettings
    {
        [Display(Name = "Email To")]
        public string EmailTo { get; set; }

        [Display(Name = "Subject Prefix")]
        public string EmailSubjectPrefix { get; set; }

        [Display(Name = "Smtp Address")]
        public string SmtpAddress { get; set; }

        [Display(Name = "Smtp Port")]
        public int SmtpPort { get; set; }

        [Display(Name = "Smtp UserName")]
        public string SmtpUserName { get; set; }

        [Display(Name = "Smtp Password")]
        public string SmtpPassword { get; set; }

        [Display(Name = "Enable SSL")]
        public bool SmtpSsl { get; set; }
    }
}