using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminSettingsSocialNetworks
    {
        [Display(Name = "Enable Facebook Like Posts")]
        public bool EnableFacebookLikePosts { get; set; }

        [Display(Name = "Enable Google Plus One Posts")]
        public bool EnableGooglePlusOnePosts { get; set; }

        [Display(Name = "Enable Twitter Share Posts")]
        public bool EnableTwitterSharePosts { get; set; }
    }
}