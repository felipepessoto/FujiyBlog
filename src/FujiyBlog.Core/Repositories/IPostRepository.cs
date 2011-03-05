using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;

namespace FujiyBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<PostSummary> GetRecentPosts(int skip, int take, string tag = null, string category = null, string authorUserName = null, bool isPublic = true);
        int GetTotal(string tag = null, string category = null, string authorUserName = null, bool isPublic = true);
        Post GetPost(string slug, bool isPublic = true);
        Post GetPost(int id, bool isPublic = true);
        Post GetPreviousPost(Post post, bool isPublic = true);
        Post GetNextPost(Post post, bool isPublic = true);
        IEnumerable<PostSummary> GetArchive(bool isPublic = true);
        IEnumerable<Category> GetCategories();
    }
}
