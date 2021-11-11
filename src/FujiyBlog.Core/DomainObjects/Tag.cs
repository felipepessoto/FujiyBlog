using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Tag
    {
        public Tag()
        {
            PostTags = new List<PostTag>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<PostTag> PostTags { get; set; }
    }
}
