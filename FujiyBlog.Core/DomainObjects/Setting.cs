using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Setting
    {
        public virtual int Id { get; set; }

        [Required, StringLength(50)]
        public virtual string Name { get; set; }

        [Required, StringLength(500)]
        public virtual string Description { get; set; }

        [Required]
        public virtual string Value { get; set; }
    }
}
