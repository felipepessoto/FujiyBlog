using System;
using System.Data.Entity;
using System.Globalization;
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

        private string lastDatabaseChange;
        public string LastDatabaseChange
        {
            get
            {
                return lastDatabaseChange ??
                       (lastDatabaseChange = new FujiyBlogDatabase().Settings.Where(x => x.Id == 24).Select(x => x.Value).SingleOrDefault() ?? "0");
            }
        }

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
            builder.Configurations.Add(new PageConfiguration());
            builder.Configurations.Add(new RoleGroupConfiguration());
        }

        public override int SaveChanges()
        {
            return SaveChanges();
        }

        public int SaveChanges(bool updateLastDbChange = true, bool bypassValidation = false)
        {
            int saveChanges = bypassValidation ? SaveChangesBypassingValidation() : base.SaveChanges();

            if (updateLastDbChange && saveChanges > 0)
            {
                UpdateLastDbChange();
            }

            return saveChanges;
        }

        public void UpdateLastDbChange()
        {
            Setting setting = Settings.SingleOrDefault(x => x.Id == 24);
            if (setting == null)
            {
                setting = new Setting {Id = 24, Description = "Last Database Change"};
                Settings.Add(setting);
            }
            setting.Value = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
            base.SaveChanges();
        }

        [Obsolete]
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
