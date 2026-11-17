using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminFeedSettings
    {
        [Display(Name = "Alternate Feed Url")]
        public string? AlternateFeedUrl { get; set; }

        [Required, Display(Name = "Items Shown In Feed")]
        [Range(0, int.MaxValue)]
        public int ItemsShownInFeed { get; set; }
    }
}