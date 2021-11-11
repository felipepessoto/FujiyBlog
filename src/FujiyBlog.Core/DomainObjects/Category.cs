using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class Category
    {
        public Category()
        {
            PostCategories = new List<PostCategory>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<PostCategory> PostCategories { get; set; }
    }
}
