using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;

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

        public static IQueryable<Post> WhereIsPublicPost(this IQueryable<Post> query)
        {
            return query.Where(post => post.IsPublished && post.PublicationDate < UtcNow);
        }
    }
}
