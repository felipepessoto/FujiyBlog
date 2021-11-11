using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FujiyBlog.Core.EntityFramework
{
    public class FujiyBlogDatabase : IdentityDbContext<ApplicationUser>
    {
        public FujiyBlogDatabase(DbContextOptions<FujiyBlogDatabase> options)
            : base(options)
        {
        }

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        //public DbSet<PostRevision> PostRevisions { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WidgetSetting> WidgetSettings { get; set; }
        public DbSet<Page> Pages { get; set; }

        private string lastDatabaseChange;
        public string LastDatabaseChange
        {
            get
            {
                return lastDatabaseChange
                    ?? (lastDatabaseChange = this.Settings.Where(x => x.Id == 24).Select(x => x.Value).SingleOrDefault() ?? "0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().HasIndex(x => x.Slug).IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(b => b.AuthoredPostComments)
                .WithOne(b => b.Author);


            modelBuilder.Entity<ApplicationUser>()
            .HasMany(b => b.ModeratedPostComments)
            .WithOne(b => b.ModeratedBy);

            modelBuilder.Entity<PostTag>()
                .HasKey(t => new { t.PostId, t.TagId });

            modelBuilder.Entity<PostCategory>()
                .HasKey(t => new { t.PostId, t.CategoryId });

            base.OnModelCreating(modelBuilder);
        }


        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            var hasChanges = ChangeTracker.Entries().Any();
            int saveChanges = 0;

            if (hasChanges)
            {
                UpdateLastDbChange();
                saveChanges = base.SaveChanges();
            }

            return saveChanges;
        }

        public void UpdateLastDbChange()
        {
            Setting setting = Settings.SingleOrDefault(x => x.Id == 24);
            if (setting == null)
            {
                setting = new Setting { Id = 24, Description = "Last Database Change" };
                Settings.Add(setting);
            }
            lastDatabaseChange = setting.Value = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
        }
    }
}
