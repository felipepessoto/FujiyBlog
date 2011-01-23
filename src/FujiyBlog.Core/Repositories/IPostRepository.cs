using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetRecentPosts(int skip, int take);
        Post GetPost(string slug);
        Post GetPost(int id);
    }
}
