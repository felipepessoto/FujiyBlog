using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Services;

namespace FujiyBlog.Core.EntityFramework
{
    public class PostRepository : RepositoryBase<Post>
    {
        public PostRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        private Post GetPost(string slug, int? id)
        {
            IQueryable<Post> posts = Database.Posts.WhereHavePermissions().Include(x => x.Author);

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

            if (post == null)
            {
                return null;
            }

            Database.Entry(post).Collection(x=>x.Tags).Load();
            Database.Entry(post).Collection(x => x.Categories).Load();
            Database.Entry(post).Collection(x => x.Comments).Query().WhereHavePermissions().Include(x => x.Author).Load();

            return post;
        }

        public IEnumerable<PostSummary> GetRecentPosts(int skip, int take, string tag = null, string category = null, string authorUserName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<Post> posts = Database.Posts.AsNoTracking().WhereHavePermissions().OrderByDescending(x => x.PublicationDate).Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories);

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
            
            if (skip > 0)
            {
                posts = posts.Skip(skip);
            }

            if (startDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate <= endDate.Value);
            }

            posts = posts.Take(take);

            Dictionary<int, int> counts = GetPostsCounts(posts);

            var postSummaries = (from post in posts.ToList()
                                  select new PostSummary
                                             {
                                                 Post = post,
                                                 CommentsTotal = counts[post.Id]
                                             }).ToList();

            return postSummaries;
        }

        private Dictionary<int, int> GetPostsCounts(IQueryable<Post> posts)
        {
            bool publicComments = RolesService.UserHasPermission(Permission.ViewPublicComments);
            bool unmoderatedComments = RolesService.UserHasPermission(Permission.ViewUnmoderatedComments);

            if (publicComments && unmoderatedComments)
            {
                return (from post in posts
                          select new { post.Id, C = post.Comments.Count(x => x.IsDeleted == false) }).ToDictionary(e => e.Id, e => e.C);
            }

            if (publicComments)
            {
                return (from post in posts
                        select new {post.Id, C = post.Comments.Count(x => x.IsApproved && x.IsDeleted == false)}).ToDictionary(
                            e => e.Id, e => e.C);
            }

            if (unmoderatedComments)
            {
                return (from post in posts
                        select new { post.Id, C = post.Comments.Count(x => x.IsApproved == false && x.IsDeleted == false) }).ToDictionary(
            e => e.Id, e => e.C);
            }

            return (from post in posts
                    select new { post.Id, C = 0 }).ToDictionary(e => e.Id, e => e.C);
        }

        public IEnumerable<PostSummary> GetArchive()
        {
            IQueryable<Post> posts = Database.Posts.WhereHavePermissions().OrderByDescending(x => x.PublicationDate).Include(x => x.Categories);

            Dictionary<int, int> counts = GetPostsCounts(posts);

            var postSummaries = (from post in posts.ToList()
                                 select new PostSummary
                                            {
                                                Post = post,
                                                CommentsTotal = counts[post.Id]
                                            }).ToList();

            return postSummaries;
        }

        public int GetTotal(string tag = null, string category = null, string authorUserName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<Post> posts = Database.Posts.WhereHavePermissions();

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

            if (startDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate <= endDate.Value);
            }

            return posts.Count();
        }

        public Post GetPost(string slug)
        {
            return GetPost(slug, null);
        }

        public Post GetPost(int id)
        {
            return GetPost(null, id);
        }

        public Post GetPreviousPost(Post post, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.WhereHavePermissions().OrderByDescending(x => x.PublicationDate).Where(x => x.PublicationDate <= post.PublicationDate && x.Id != post.Id);

            return posts.FirstOrDefault();
        }

        public Post GetNextPost(Post post, bool isPublic = true)
        {
            IQueryable<Post> posts = Database.Posts.WhereHavePermissions().OrderBy(x => x.PublicationDate).Where(x => x.PublicationDate >= post.PublicationDate && x.Id != post.Id);

            return posts.FirstOrDefault();
        }

        public IEnumerable<Category> GetCategories()
        {
            return Database.Categories.OrderBy(x => x.Name).ToList();
        }

        public IEnumerable<TagWithCount> GetTagsCloud(int minimumPosts)
        {
            var tags = from tag in Database.Tags
                       where tag.Posts.Count() >= minimumPosts
                       orderby tag.Name
                       select new TagWithCount
                                  {
                                      Tag = tag,
                                      PostsCount = tag.Posts.Count()
                                  };

            return tags.ToList();
        }

        public IEnumerable<Tuple<DateTime, int>> GetArchiveCountByMonth(bool descending)
        {
            var months = Database.Posts.WhereHavePermissions().GroupBy(data => new {data.PublicationDate.Year, data.PublicationDate.Month});

            if(descending)
            {
                months = months.OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month);
            }
            else
            {
                months = months.OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);
            }

            var monthsProj = months.Select(g => new
                                                    {
                                                        Data = g.Key,
                                                        Count = g.Count()
                                                    }).ToList();

            var monthsTuples = from data in monthsProj
                               select Tuple.Create(new DateTime(data.Data.Year, data.Data.Month, 1), data.Count);

            return monthsTuples;
        }

        public IEnumerable<Tag> GetOrCreateTags(IEnumerable<string> tagsNames)
        {
            var tags = (from tag in Database.Tags
                       where tagsNames.Contains(tag.Name)
                       select tag).ToList();

            var upperTags = tags.Select(x => x.Name.ToUpperInvariant());
            var tagsNotFound = from tag in tagsNames
                               where !upperTags.Contains(tag.ToUpperInvariant())
                               select tag;

            foreach (string newTagName in tagsNotFound)
            {
                Tag newTag = new Tag();
                newTag.Name = newTagName;
                tags.Add(Database.Tags.Add(newTag));
            }
            return tags;
        }
    }
}
