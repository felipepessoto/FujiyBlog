using FujiyBlog.Core.DomainObjects;
using System.Collections.Generic;

namespace FujiyBlog.Web.ViewModels
{
    public class WidgetIndex
    {
        public IEnumerable<WidgetSetting> WidgetSettings { get; set; }
        public IEnumerable<string> AvailableWidgets { get; set; }

        public string InsertedWidget { get; set; }
        public string ZoneName { get; set; }
    }
}