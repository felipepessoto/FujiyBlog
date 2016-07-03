using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
{
    public class BlogMLRepository
    {
        private readonly FujiyBlogDatabase database;

        public BlogMLRepository(FujiyBlogDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            this.database = database;
        }

        public void AddPost(Post post)
        {
            database.Posts.Add(post);
        }

        public void AddCategory(Category category)
        {
            database.Categories.Add(category);
        }

        public void AddTag(Tag tag)
        {
            database.Tags.Add(tag);
        }

        public void AddUser(ApplicationUser user)
        {
            database.Users.Add(user);
        }

        public List<Category> GetAllCategories()
        {
            return database.Categories.ToList();
        }

        public List<Tag> GetAllTags()
        {
            return database.Tags.ToList();
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return database.Users.ToList();
        }

        public Category GetCategory(string categoryName)
        {
            return database.Categories.SingleOrDefault(x => x.Name == categoryName);
        }

        public Tag GetTag(string tagName)
        {
            return database.Tags.SingleOrDefault(x => x.Name == tagName);
        }

        public ApplicationUser GetUser(string userName)
        {
            return database.Users.SingleOrDefault(x => x.UserName == userName);
        }

        public Post GetPost(string slug)
        {
            return database.Posts.SingleOrDefault(x => x.Slug == slug);
        }
    }
}
