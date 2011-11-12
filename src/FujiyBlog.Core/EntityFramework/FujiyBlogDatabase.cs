using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework.Configuration;

namespace FujiyBlog.Core.EntityFramework
{
    public class FujiyBlogDatabase : DbContext
    {
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WidgetSetting> WidgetSettings { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<RoleGroup> RoleGroups { get; set; }

        private static string lastCache;
        public string LastCache
        {
            get { return lastCache; }
        }

        public FujiyBlogDatabase()
        {
            Database.SetInitializer(new FujiyBlogDatabaseInitializer());

            string lastCacheAtDb = Settings.Single(x => x.Id == 24).Value;
            if (lastCacheAtDb != lastCache)
            {
                lastCache = lastCacheAtDb;
                CacheHelper.ClearCache();
            }
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Conventions.Remove<IncludeMetadataConvention>();

            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new SettingConfiguration());
            builder.Configurations.Add(new TagConfiguration());
            builder.Configurations.Add(new CategoryConfiguration());
            builder.Configurations.Add(new WidgetSettingConfiguration());
            builder.Configurations.Add(new PageConfiguration());
            builder.Configurations.Add(new RoleGroupConfiguration());
        }

        public override int SaveChanges()
        {
            int saveChanges = base.SaveChanges();

            if (saveChanges > 0)
            {
                Settings.Single(x => x.Id == 24).Value = DateTime.UtcNow.Ticks.ToString();
                base.SaveChanges();
            }

            return saveChanges;
        }

        public int SaveChangesBypassingValidation()
        {
            bool previousValue = Configuration.ValidateOnSaveEnabled;

            try
            {
                Configuration.ValidateOnSaveEnabled = false;
                return SaveChanges();
            }
            finally
            {
                Configuration.ValidateOnSaveEnabled = previousValue;
            }
        }
    }
}
