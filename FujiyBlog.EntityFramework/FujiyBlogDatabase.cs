using System.Data.Edm.Db.Mapping;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.EntityFramework.Configuration;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabase : DbContext
    {
        public FujiyBlogDatabase(string connectionString)
            : base(CriarModel(connectionString))
        { }

        public static DbModel CriarModel(string connectionString)
        {
            ModelBuilder builder = new ModelBuilder();
            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());

            DbDatabaseMapping dbm = builder.Build(new SqlConnection(connectionString));

            return  new DbModel(dbm);
        }

        //public string Script()
        //{
        //    return ObjectContext.CreateDatabaseScript();
        //}

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
