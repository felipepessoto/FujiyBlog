using System;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
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

        public static IQueryable<Post> WhereHavePermissions(this IQueryable<Post> query)
        {
            bool publicPost = RolesService.UserHasPermission(Permission.ViewPublicPosts);
            bool unpublishedPost = RolesService.UserHasPermission(Permission.ViewUnpublishedPosts);

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

        public static IQueryable<PostComment> WhereHavePermissions(this IQueryable<PostComment> query)
        {
            bool publicComments = RolesService.UserHasPermission(Permission.ViewPublicComments);
            bool unmoderatedComments = RolesService.UserHasPermission(Permission.ViewUnmoderatedComments);

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

        public static IQueryable<Page> WhereHavePermissions(this IQueryable<Page> query)
        {
            bool publicPages = RolesService.UserHasPermission(Permission.ViewPublicPages);
            bool unpublishedPages = RolesService.UserHasPermission(Permission.ViewUnpublishedPages);

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
    }
}
