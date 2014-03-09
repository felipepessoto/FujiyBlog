using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Setting
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; }

        public string Value { get; set; }
    }
}
