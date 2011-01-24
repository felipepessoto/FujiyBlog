using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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

        private static readonly Expression<Func<Post, bool>> PublicPost = x => x.IsPublished && !x.IsDeleted && x.PublicationDate < DateTime.Now;

        public IEnumerable<Post> GetRecentPosts(bool isPublic, int skip, int take)
        {
            IQueryable<Post> posts = Database.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Tags).OrderByDescending(x => x.PublicationDate);

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            if (skip > 0)
            {
                posts = posts.Skip(skip);
            }

            posts = posts.Take(take);

            return posts.ToList();
        }

        public Post GetPost(string slug)
        {
            return Database.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Tags).SingleOrDefault(x => x.Slug == slug);
        }

        public Post GetPost(int id)
        {
            return Database.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Tags).SingleOrDefault(x => x.Id == id);
        }
    }
}
