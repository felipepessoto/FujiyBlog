using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetRecentPosts(int skip, int take, string tag = null, string category = null, bool isPublic = true);
        int GetTotal(string tag = null, string category = null, bool isPublic = true);
        Post GetPost(string slug, bool isPublic = true);
        Post GetPost(int id, bool isPublic = true);
        Post GetPreviousPost(Post post, bool isPublic = true);
        Post GetNextPost(Post post, bool isPublic = true);
        IEnumerable<Post> GetArchive(bool isPublic = true);
    }
}
