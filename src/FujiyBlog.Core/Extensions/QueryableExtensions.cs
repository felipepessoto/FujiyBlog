using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace FujiyBlog.Core.Extensions
{
    public static class QueryableExtensions
    {
        private static DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int currentPage, int pageSize)
        {
            if (currentPage > 1)
            {
                query = query.Skip((currentPage - 1)*pageSize);
            }

            return query.Take(pageSize);
        }

        public static IQueryable<Post> WhereHaveRoles(this IQueryable<Post> query, HttpContext httpContext)
        { 
            bool publicPost = httpContext.UserHasClaimPermission(PermissionClaims.ViewPublicPosts);
            bool unpublishedPost = httpContext.UserHasClaimPermission(PermissionClaims.ViewUnpublishedPosts);

            if (publicPost && unpublishedPost)
            {
                return query.Where(x => x.IsDeleted == false);
            }

            if (publicPost)
            {
                return query.Where(x => x.IsDeleted == false && x.IsPublished && x.PublicationDate <= UtcNow);
            }

            if (unpublishedPost)
            {
                return query.Where(x => x.IsDeleted == false && !x.IsPublished || x.PublicationDate > UtcNow);
            }

            return query.Where(x => false);
        }

        public static IQueryable<PostComment> WhereHaveRoles(this IQueryable<PostComment> query, HttpContext httpContext)
        {
            bool publicComments = httpContext.UserHasClaimPermission(PermissionClaims.ViewPublicComments);
            bool unmoderatedComments = httpContext.UserHasClaimPermission(PermissionClaims.ViewUnmoderatedComments);

            if (publicComments && unmoderatedComments)
            {
                return query.Where(x => x.IsDeleted == false);
            }

            if (publicComments)
            {
                return query.Where(x => x.IsDeleted == false && x.IsApproved);
            }

            if (unmoderatedComments)
            {
                return query.Where(x => x.IsDeleted == false && x.IsApproved == false);
            }

            return query.Where(x => false);
        }

        public static IQueryable<Page> WhereHaveRoles(this IQueryable<Page> query, HttpContext httpContext)
        {
            bool publicPages = httpContext.UserHasClaimPermission(PermissionClaims.ViewPublicPages);
            bool unpublishedPages = httpContext.UserHasClaimPermission(PermissionClaims.ViewUnpublishedPages);

            if (publicPages && unpublishedPages)
            {
                return query.Where(x=> x.IsDeleted == false);
            }

            if (publicPages)
            {
                return query.Where(x => x.IsDeleted == false && x.IsPublished && x.PublicationDate <= UtcNow);
            }

            if (unpublishedPages)
            {
                return query.Where(x => x.IsDeleted == false && !x.IsPublished || x.PublicationDate > UtcNow);
            }

            return query.Where(x => false);
        }

        public static Post GetPreviousPost(this IQueryable<Post> query, Post post, HttpContext httpContext)
        {
            return query.WhereHaveRoles(httpContext).OrderByDescending(x => x.PublicationDate).FirstOrDefault(x => x.PublicationDate <= post.PublicationDate && x.Id != post.Id);
        }

        public static Post GetNextPost(this IQueryable<Post> query, Post post, HttpContext httpContext)
        {
            return query.WhereHaveRoles(httpContext).OrderBy(x => x.PublicationDate).FirstOrDefault(x => x.PublicationDate >= post.PublicationDate && x.Id != post.Id);
        }

        public static IEnumerable<TagWithCount> GetTagsCloud(this IQueryable<Tag> query, int minimumPosts)
        {
            var tags = from tag in query
                       where tag.PostTags.Count() >= minimumPosts
                       orderby tag.Name
                       select new TagWithCount
                       {
                           Tag = tag,
                           PostsCount = tag.PostTags.Count()
                       };

            return tags.ToList();
        }

        public static Post GetCompletePost(this FujiyBlogDatabase database, string slug, HttpContext httpContext)
        {
            Post post = database.Posts.WhereHaveRoles(httpContext).Include(x => x.Author).SingleOrDefault(x => x.Slug == slug);

            if (post == null)
            {
                return null;
            }

            database.Tags.Include(x=>x.PostTags).Where(x => x.PostTags.Any(y => y.PostId == post.Id)).Load();
            database.Categories.Include(x=>x.PostCategories).Where(x => x.PostCategories.Any(y => y.PostId == post.Id)).Load();
            database.PostComments.Include(x=>x.Author).WhereHaveRoles(httpContext).Load();

            return post;
        }
    }
}
