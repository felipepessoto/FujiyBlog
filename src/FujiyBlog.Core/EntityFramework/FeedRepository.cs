using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
{
    public class FeedRepository
    {
        private readonly FujiyBlogDatabase database;

        public FeedRepository(FujiyBlogDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            this.database = database;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return database.Users.ToList();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return database.Categories.ToList();
        }

        public IEnumerable<Post> GetPosts(int take)
        {
            DateTime utcNow = DateTime.UtcNow;

            return (from post in database.Posts
                    where !post.IsDeleted && post.IsPublished && post.PublicationDate < utcNow
                   orderby post.PublicationDate descending
                   select post).Take(take).ToList();
        }
    }
}
