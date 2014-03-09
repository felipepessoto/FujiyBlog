using System.Collections.Generic;
using BlogML.Xml;

namespace FujiyBlog.Core.BlogML
{
    public class BlogMLExtendedPost
    {
        public BlogMLPost Post { get; private set; }
        public IEnumerable<string> Tags { get; private set; }

        public BlogMLExtendedPost(BlogMLPost post, IEnumerable<string> tags)
        {
            Post = post;
            Tags = tags;
        }
    }
}
