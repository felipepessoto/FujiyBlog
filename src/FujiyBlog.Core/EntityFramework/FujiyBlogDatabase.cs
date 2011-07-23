using System.Data.Entity;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework.Configuration;

namespace FujiyBlog.Core.EntityFramework
{
    public class FujiyBlogDatabase : DbContext
    {
        public FujiyBlogDatabase()
        {
            Database.SetInitializer(new FujiyBlogDatabaseInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new SettingConfiguration());
            builder.Configurations.Add(new TagConfiguration());
            builder.Configurations.Add(new CategoryConfiguration());
            builder.Configurations.Add(new WidgetSettingConfiguration());
        }

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WidgetSetting> WidgetSettings { get; set; }
    }
}
