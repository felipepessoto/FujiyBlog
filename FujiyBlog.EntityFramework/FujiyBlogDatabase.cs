using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabase : DbContext
    {
        public FujiyBlogDatabase(DbModel model)
            : base(model)
        { }

        //public string Script()
        //{
        //    return ObjectContext.CreateDatabaseScript();
        //}

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
