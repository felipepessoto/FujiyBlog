using System.Data.Edm.Db.Mapping;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using FujiyBlog.EntityFramework.Configuration;

namespace FujiyBlog.EntityFramework
{
    internal class FujiyBlogDatabaseBuilder
    {
        public static FujiyBlogDatabase Criar(string connectionString)
        {
            ModelBuilder builder = new ModelBuilder();
            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());

            DbDatabaseMapping dbm = builder.Build(new SqlConnection(connectionString));

            DbModel model = new DbModel(dbm);

            return new FujiyBlogDatabase(model);
        }
    }
}
