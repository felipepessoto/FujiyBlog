using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Setting
    {
        [Required]
        public virtual int Id { get; set; }

        [Required, StringLength(500)]
        public virtual string Description { get; set; }

        [Required]
        public virtual string Value { get; set; }
    }
}
