using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.ViewModel;

namespace FujiyBlog.EntityFramework
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public IEnumerable<PostDetails> GetRecentPosts(int skip, int take)
        {
            IQueryable<Post> posts = Database.Posts.Include("Author").Include("Tags").Include("Comments");

            if (skip > 0)
            {
                posts = posts.Skip(skip);
            }
            IEnumerable<PostDetails> postDetails = from post in posts.Take(take)
                                                   select new PostDetails
                                                              {
                                                                  Post = post,
                                                                  AuthorDisplayName = post.Author.DisplayName,
                                                                  AuthorLogin = post.Author.Login,
                                                                  Tags = post.Tags,
                                                                  CommentsCount = post.Comments.Count(x=> x.IsApproved && !x.IsDeleted)
                                                              };

            return postDetails.ToList();
        }

        public Post GetPost(string slug)
        {
            return Database.Posts.SingleOrDefault(x => x.Slug == slug);
        }
    }
}
