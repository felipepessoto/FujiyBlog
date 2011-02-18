using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.EntityFramework
{
    public class FeedRepository : IFeedRepository
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
            return (from post in database.Posts
                   where !post.IsDeleted && post.IsPublished && post.PublicationDate < DateTime.UtcNow
                   orderby post.PublicationDate descending
                   select post).Take(take).ToList();
        }
    }
}
