using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FujiyBlog.Core.DomainObjects
{
    public class Setting
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string? Value { get; set; }
    }
}
