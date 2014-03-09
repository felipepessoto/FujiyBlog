using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        [Display(Name = "Custom Code"), AllowHtml, DataType(DataType.MultilineText)]
        public string CustomCode { get; set; }

        public IEnumerable<SelectListItem> Themes { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        private static List<SelectListItem> languagesCache;
        public List<SelectListItem> Languages
        {
            get
            {
                if (languagesCache == null)
                {
                    List<SelectListItem> languages = new List<SelectListItem> { new SelectListItem { Text = "Auto", Value = "Auto" }, new SelectListItem { Text = "English", Value = "en" } };
                    foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (Resources.Labels.ResourceManager.GetResourceSet(culture, false, false) != null)
                        {
                            languages.Add(new SelectListItem { Text = culture.DisplayName, Value = culture.Name });
                        }
                    }
                    languagesCache = languages;
                }
                return languagesCache;
            }
        }
    }
}