using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FujiyBlog.Core.Dto;
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
                int idInt = id.GetValueOrDefault();
                post = posts.SingleOrDefault(x => x.Id == idInt);
            }
            else
            {
                post = posts.SingleOrDefault(x => x.Slug == slug);
            }

            Database.Entry(post).Collection(x=>x.Tags).Load();
            Database.Entry(post).Collection(x => x.Categories).Load();
            if (isPublic)
            {
                Database.Entry(post).Collection(x => x.Comments).Query().Include(x => x.Author).Where(PublicPostComment).Load();
            }
            else
            {
                Database.Entry(post).Collection(x => x.Comments).Query().Include(x => x.Author).Load();
            }

            return post;
        }

        public IEnumerable<PostSummary> GetRecentPosts(int skip, int take, string tag = null, string category = null, string authorUserName = null, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts;
            IQueryable<PostComment> comments = Database.PostComments;

            if (tag != null)
            {
                posts = posts.Where(x => x.Tags.Any(y => y.Name == tag));
            }

            if (category != null)
            {
                posts = posts.Where(x => x.Categories.Any(y => y.Name == category));
            }

            if (authorUserName != null)
            {
                posts = posts.Where(x => x.Author.Username == authorUserName);
            }

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
                comments = comments.Where(PublicPostComment);
            }

            if (skip > 0)
            {
                posts = posts.Skip(skip);
            }

            posts = posts.Take(take);

            var postSummaries = ((from post in posts
                                  join comment in comments on post.Id equals comment.Post.Id into g
                                  select new PostSummary
                                             {
                                                 Post = post,
                                                 Author = post.Author,
                                                 Tags = post.Tags,
                                                 Categories = post.Categories,
                                                 CommentsTotal = g.Count()
                                             }).OrderByDescending(x => x.Post.PublicationDate).ToList());

            return postSummaries;
        }

        public IEnumerable<PostSummary> GetArchive(bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts;
            IQueryable<PostComment> comments = Database.PostComments;

            if (isPublic)
            {
                posts = posts.Where(PublicPost);
                comments = comments.Where(PublicPostComment);
            }

            var postSummaries = ((from post in posts
                                  join comment in comments on post.Id equals comment.Post.Id into g
                                  select new PostSummary
                                  {
                                      Post = post,
                                      Categories = post.Categories,
                                      CommentsTotal = g.Count()
                                  }).OrderByDescending(x => x.Post.PublicationDate).ToList());

            return postSummaries;
        }

        public int GetTotal(string tag = null, string category = null, string authorUserName = null, bool isPublic = true)
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

            if (authorUserName != null)
            {
                posts = posts.Where(x => x.Author.Username == authorUserName);
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

        public IEnumerable<Category> GetCategories()
        {
            return Database.Categories.OrderBy(x => x.Name).ToList();
        }
    }
}
