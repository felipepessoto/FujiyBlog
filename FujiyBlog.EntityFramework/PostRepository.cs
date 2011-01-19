using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public IEnumerable<Post> GetRecentPosts(int skip, int take)
        {
            if (skip > 0)
            {
                return Database.Posts.Skip(skip).Take(take);
            }
            return Database.Posts.Take(take);
        }

        public Post GetPost(string slug)
        {
            return Database.Posts.SingleOrDefault(x => x.Slug == slug);
        }
    }
}
