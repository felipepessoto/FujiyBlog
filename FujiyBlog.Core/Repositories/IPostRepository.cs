using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetRecentPosts(int skip, int take);
        Post GetPost(string slug);
    }
}
