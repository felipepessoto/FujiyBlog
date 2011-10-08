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
    }
}
