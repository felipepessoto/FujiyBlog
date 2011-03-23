using System.Data.Entity;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabaseInitializer : CreateDatabaseIfNotExists<FujiyBlogDatabase>
    {
        protected override void Seed(FujiyBlogDatabase context)
        {
            Setting minRequiredPasswordLength = new Setting
                                                    {
                                                        Id = 1,
                                                        Description = "MinRequiredPasswordLength",
                                                        Value = "6"
                                                    };

            Setting postsPerPage = new Setting
            {
                Id = 2,
                Description = "PostsPerPage",
                Value = "10"
            };

            Setting blogName = new Setting
            {
                Id = 3,
                Description = "BlogName",
                Value = "Your Name"
            };

            Setting blogDescription = new Setting
            {
                Id = 4,
                Description = "BlogDescription",
                Value = "BlogDescription"
            };

            Setting theme = new Setting
            {
                Id = 5,
                Description = "Theme",
                Value = "Standand"
            };

            context.Settings.Add(minRequiredPasswordLength);
            context.Settings.Add(postsPerPage);
            context.Settings.Add(blogName);
            context.Settings.Add(blogDescription);
            context.Settings.Add(theme);

            base.Seed(context);
        }
    }
}
