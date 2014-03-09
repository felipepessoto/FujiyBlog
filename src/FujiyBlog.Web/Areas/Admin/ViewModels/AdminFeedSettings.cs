using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminFeedSettings
    {
        [DisplayName("Alternate Feed Url")]
        public string AlternateFeedUrl { get; set; }

        [Required, DisplayName("Items Shown In Feed")]
        public int ItemsShownInFeed { get; set; }

        [Required, DisplayName("Default Feed Output")]
        public string DefaultFeedOutput { get; set; }

        public IEnumerable<SelectListItem> DefaultFeedOutputs { get; set; }
    }
}