using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class WidgetSetting
    {
        [Required]
        public int Id { get; set; }

        [Required,StringLength(50)]
        public string Name { get; set; }

        [Required,StringLength(50)]
        public string WidgetZone { get; set; }

        public int Position { get; set; }

        public string Settings { get; set; }
    }
}
