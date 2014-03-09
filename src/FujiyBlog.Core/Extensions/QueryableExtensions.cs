using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;

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

        public static IQueryable<Post> WhereHaveRoles(this IQueryable<Post> query)
        { 
            bool publicPost = RolesService.UserHasRole(Role.ViewPublicPosts);
            bool unpublishedPost = RolesService.UserHasRole(Role.ViewUnpublishedPosts);

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

        public static IQueryable<PostComment> WhereHaveRoles(this IQueryable<PostComment> query)
        {
            bool publicComments = RolesService.UserHasRole(Role.ViewPublicComments);
            bool unmoderatedComments = RolesService.UserHasRole(Role.ViewUnmoderatedComments);

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

        public static IQueryable<Page> WhereHaveRoles(this IQueryable<Page> query)
        {
            bool publicPages = RolesService.UserHasRole(Role.ViewPublicPages);
            bool unpublishedPages = RolesService.UserHasRole(Role.ViewUnpublishedPages);

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

        public static Post GetPreviousPost(this IQueryable<Post> query, Post post)
        {
            return query.WhereHaveRoles().OrderByDescending(x => x.PublicationDate).FirstOrDefault(x => x.PublicationDate <= post.PublicationDate && x.Id != post.Id);
        }

        public static Post GetNextPost(this IQueryable<Post> query, Post post)
        {
            return query.WhereHaveRoles().OrderBy(x => x.PublicationDate).FirstOrDefault(x => x.PublicationDate >= post.PublicationDate && x.Id != post.Id);
        }

        public static IEnumerable<TagWithCount> GetTagsCloud(this IQueryable<Tag> query, int minimumPosts)
        {
            var tags = from tag in query
                       where tag.Posts.Count() >= minimumPosts
                       orderby tag.Name
                       select new TagWithCount
                       {
                           Tag = tag,
                           PostsCount = tag.Posts.Count()
                       };

            return tags.ToList();
        }

        public static Post GetCompletePost(this FujiyBlogDatabase database, string slug)
        {
            Post post = database.Posts.WhereHaveRoles().Include(x => x.Author).SingleOrDefault(x => x.Slug == slug);

            if (post == null)
            {
                return null;
            }

            database.Entry(post).Collection(x => x.Tags).Load();
            database.Entry(post).Collection(x => x.Categories).Load();
            database.Entry(post).Collection(x => x.Comments).Query().WhereHaveRoles().Include(x => x.Author).Load();

            return post;
        }
    }
}
