using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Dto
{
    public class TagWithCount
    {
        public Tag Tag { get; set; }
        public int PostsCount { get; set; }
    }
}
