using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminBasicSettings
    {
        [Required]
        [Display(Name = "Blog Name")]
        public string BlogName { get; set; }

        [Display(Name = "Blog Description")]
        public string BlogDescription { get; set; }

        [Required]
        public string Theme { get; set; }

        [Display(Name = "Posts Per Page")]
        public int PostsPerPage { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }

        [Required]
        [Display(Name = "Language")]
        public string Language { get; set; }

        public IEnumerable<SelectListItem> Themes { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public List<SelectListItem> Languages { get; set; }
    }
}