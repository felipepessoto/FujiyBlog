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

        [Display(Name = "Facebook Admin Ids")]
        public string FacebookAdminIds { get; set; }

        [Display(Name = "Facebook App ID")]
        public string FacebookAppId { get; set; }

        [Display(Name = "Default Open Graph Image(used on facebook share)")]
        public string OpenGraphImageUrl { get; set; }
    }
}