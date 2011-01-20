using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.ViewModel
{
    public class PostDetails
    {
        public Post Post;
        public string AuthorDisplayName;
        public string AuthorLogin;
        public IEnumerable<Tag> Tags;
        public int CommentsCount;
    }
}
