using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Dto
{
    public class PostSummary
    {
        public Post Post { get; set; }
        public int CommentsTotal { get; set; }
        public User Author { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
