using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FujiyBlog.Core.DomainObjects
{
    public class PostCategory
    {
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
