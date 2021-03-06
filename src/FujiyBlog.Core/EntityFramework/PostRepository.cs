﻿using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FujiyBlog.Core.EntityFramework
{
    public class PostRepository : RepositoryBase<Post>
    {
        private readonly IHttpContextAccessor contextAccessor;

        public PostRepository(FujiyBlogDatabase database, IHttpContextAccessor contextAccessor)
            : base(database)
        {
            this.contextAccessor = contextAccessor;
        }

        public IEnumerable<PostSummary> GetRecentPosts(int skip, int take, string tag = null, string category = null, string authorUserName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<Post> posts = Database.Posts.AsNoTracking().WhereHaveRoles(contextAccessor.HttpContext).Include(x => x.Author).Include(x => x.PostTags).ThenInclude(x=>x.Tag).Include(x => x.PostCategories).ThenInclude(x=>x.Category).OrderByDescending(x => x.PublicationDate);

            if (tag != null)
            {
                posts = posts.Where(x => x.PostTags.Any(y => y.Tag.Name == tag));
            }

            if (category != null)
            {
                posts = posts.Where(x => x.PostCategories.Any(y => y.Category.Name == category));
            }

            if (authorUserName != null)
            {
                posts = posts.Where(x => x.Author.UserName == authorUserName);
            }

            if (startDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                posts = posts.Where(x => x.PublicationDate <= endDate.Value);
            }

            Dictionary<int, int> counts = GetPostsCounts(posts, skip, take);

            var postSummaries = (from post in posts.Skip(skip).Take(take).ToList()
                                  select new PostSummary
                                             {
                                                 Post = post,
                                                 CommentsTotal = counts[post.Id]
                                             }).ToList();

            return postSummaries;
        }

        private Dictionary<int, int> GetPostsCounts(IQueryable<Post> posts, int skip, int take)
        {
            bool publicComments = contextAccessor.HttpContext.UserHasClaimPermission(PermissionClaims.ViewPublicComments);
            bool unmoderatedComments = contextAccessor.HttpContext.UserHasClaimPermission(PermissionClaims.ViewUnmoderatedComments);

            if (publicComments && unmoderatedComments)
            {
                return (from post in posts
                          select new { post.Id, C = post.Comments.Count(x => x.IsDeleted == false) }).Skip(skip).Take(take).ToDictionary(e => e.Id, e => e.C);
            }

            if (publicComments)
            {
                return (from post in posts
                        select new {post.Id, C = post.Comments.Count(x => x.IsApproved && x.IsDeleted == false)}).Skip(skip).Take(take).ToDictionary(
                            e => e.Id, e => e.C);
            }

            if (unmoderatedComments)
            {
                return (from post in posts
                        select new { post.Id, C = post.Comments.Count(x => x.IsApproved == false && x.IsDeleted == false) }).Skip(skip).Take(take).ToDictionary(
            e => e.Id, e => e.C);
            }

            return (from post in posts
                    select new { post.Id, C = 0 }).Skip(skip).Take(take).ToDictionary(e => e.Id, e => e.C);
        }

        public IEnumerable<PostSummary> GetArchive()
        {
            IQueryable<Post> posts = Database.Posts.WhereHaveRoles(contextAccessor.HttpContext).Include(x => x.PostCategories).ThenInclude(x=>x.Category).OrderByDescending(x => x.PublicationDate);

            Dictionary<int, int> counts = GetPostsCounts(posts, 0, int.MaxValue);

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
            IQueryable<Post> posts = Database.Posts.WhereHaveRoles(contextAccessor.HttpContext);

            if (tag != null)
            {
                posts = posts.Where(x => x.PostTags.Any(y => y.Tag.Name == tag));
            }

            if (category != null)
            {
                posts = posts.Where(x => x.PostCategories.Any(y => y.Category.Name == category));
            }

            if (authorUserName != null)
            {
                posts = posts.Where(x => x.Author.UserName == authorUserName);
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

        public IEnumerable<Tuple<DateTime, int>> GetArchiveCountByMonth(bool descending)
        {
            var months = Database.Posts.WhereHaveRoles(contextAccessor.HttpContext).GroupBy(data => new {data.PublicationDate.Year, data.PublicationDate.Month});

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
                tags.Add(Database.Tags.Add(newTag).Entity);
            }
            return tags;
        }
    }
}
