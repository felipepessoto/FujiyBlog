using System;

namespace FujiyBlog.Core.EntityFramework
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly FujiyBlogDatabase Database;

        protected RepositoryBase(FujiyBlogDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            Database = database;
        }

        public virtual void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Database.Set<T>().Add(entity);
        }

        public virtual void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Database.Set<T>().Remove(entity);
        }
    }
}
