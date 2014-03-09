using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminCommentsSettings
    {
        [Display(Name = "Enable Comments")]
        public bool EnableComments { get; set; }

        [Display(Name = "Moderate Comments")]
        public bool ModerateComments { get; set; }

        [Display(Name = "Enable Nested Comments")]
        public bool EnableNestedComments { get; set; }

        [Display(Name = "Close Comments After")]
        public int? CloseCommentsAfterDays { get; set; }

        [Display(Name = "Comments Per Page")]
        public int CommentsPerPage { get; set; }

        [Display(Name = "Comments Avatar")]
        public string CommentsAvatar { get; set; }

        [Display(Name = "Enable ReCaptcha")]
        public bool ReCaptchaEnabled { get; set; }

        [Display(Name = "ReCaptcha private key")]
        public string ReCaptchaPrivateKey { get; set; }

        [Display(Name = "ReCaptcha public key")]
        public string ReCaptchaPublicKey { get; set; }

        [Display(Name = "Notify new comments")]
        public bool NotifyNewComments { get; set; }
    }
}