using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminBasicSettings
    {
        [Required]
        public string BlogName { get; set; }

        public string BlogDescription { get; set; }

        [Required]
        public string Theme { get; set; }

        public int PostsPerPage { get; set; }

        [Required]
        public string TimeZoneId { get; set; }
    }
}