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

        private static readonly Expression<Func<Post, bool>> PublicPost = x => x.IsPublished && !x.IsDeleted && x.PublicationDate < DateTime.UtcNow;
        private static readonly Expression<Func<PostComment, bool>> PublicPostComment = x => x.IsApproved && !x.IsDeleted;

        private Post GetPost(string slug, int? id, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.Include(x => x.Author);

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            Post post;

            if (id.HasValue)
            {
                post = posts.SingleOrDefault(x => x.Id == id.GetValueOrDefault());
            }
            else
            {
                post = posts.SingleOrDefault(x => x.Slug == slug);
            }

            FillCommentsCount(post, isPublic);

            Database.Entry(post).Collection(x=>x.Tags).Load();
            Database.Entry(post).Collection(x => x.Categories).Load();
            if (isPublic)
            {
                Database.Entry(post).Collection(x => x.Comments).Query().Where(PublicPostComment).Load();
            }
            else
            {
                Database.Entry(post).Collection(x => x.Comments).Query().Load();
            }

            return post;
        }

        private void FillCommentsCount(Post post, bool isPublic = true)
        {
            IQueryable<PostComment> comments = Database.Entry(post).Collection(p => p.Comments).Query();

            if (isPublic)
            {
                comments = comments.Where(PublicPostComment);
            }

            post.CommentsCount = comments.Count();
        }

        public IEnumerable<Post> GetRecentPosts(int skip, int take, string tag = null, string category = null, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories).OrderByDescending(x => x.PublicationDate);

            if (tag != null)
            {
                posts = posts.Where(x => x.Tags.Any(y => y.Name == tag));
            }

            if (category != null)
            {
                posts = posts.Where(x => x.Categories.Any(y => y.Name == category));
            }

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            if (skip > 0)
            {
                posts = posts.Skip(skip);
            }

            posts = posts.Take(take);

            List<Post> postsList = posts.ToList();

            foreach (Post post in postsList)
            {
                FillCommentsCount(post, isPublic);
            }

            return postsList;
        }

        public IEnumerable<Post> GetArchive(bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.Include(x => x.Categories);

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            List<Post> postsList = posts.ToList();

            foreach (Post post in postsList)
            {
                FillCommentsCount(post, isPublic);
            }

            return postsList;
        }

        public int GetTotal(string tag = null, string category = null, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts;

            if (tag != null)
            {
                posts = posts.Where(x => x.Tags.Any(y => y.Name == tag));
            }

            if (category != null)
            {
                posts = posts.Where(x => x.Categories.Any(y => y.Name == category));
            }

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }
            return posts.Count();
        }

        public Post GetPost(string slug, bool isPublic)
        {
            return GetPost(slug, null, isPublic);
        }

        public Post GetPost(int id, bool isPublic)
        {
            return GetPost(null, id, isPublic);
        }

        public Post GetPreviousPost(Post post, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.OrderBy(x => x.PublicationDate).Where(x=>x.PublicationDate <= post.PublicationDate && x.Id != post.Id);

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            return posts.FirstOrDefault();
        }

        public Post GetNextPost(Post post, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.OrderByDescending(x => x.PublicationDate).Where(x => x.PublicationDate >= post.PublicationDate && x.Id != post.Id);

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
            }

            return posts.FirstOrDefault();
        }   
    }
}
