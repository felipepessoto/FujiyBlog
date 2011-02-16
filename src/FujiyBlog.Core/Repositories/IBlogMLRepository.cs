using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IBlogMLRepository
    {
        void AddPost(Post post);
        void AddCategory(Category category);
        void AddTag(Tag tag);
        void AddUser(User user);

        List<Category> GetAllCategories();
        List<Tag> GetAllTags();
        List<User> GetAllUsers();

        Category GetCategory(string categoryName);
        Tag GetTag(string tagName);
        User GetUser(string userName);
        Post GetPost(string slug);
    }
}
