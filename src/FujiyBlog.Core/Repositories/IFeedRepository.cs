using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IFeedRepository
    {
        IEnumerable<User> GetAllUsers();
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Post> GetPosts(int take);
    }
}
