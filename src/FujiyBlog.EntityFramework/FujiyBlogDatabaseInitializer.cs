using System;
using System.Data.Entity;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabaseInitializer : CreateDatabaseIfNotExists<FujiyBlogDatabase>
    {
        protected override void Seed(FujiyBlogDatabase context)
        {
            DateTime utcNow = DateTime.UtcNow;

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
                Value = "Default"
            };

            Setting utcOffset = new Setting
            {
                Id = 6,
                Description = "Utc Offset",
                Value = "0"
            };

            context.Settings.Add(minRequiredPasswordLength);
            context.Settings.Add(postsPerPage);
            context.Settings.Add(blogName);
            context.Settings.Add(blogDescription);
            context.Settings.Add(theme);
            context.Settings.Add(utcOffset);

            User admin = new User
                             {
                                 CreationDate = utcNow,
                                 Username = "admin",
                                 Password = "admin",
                                 Email = "admin@example.com",
                                 Enabled = true,
                             };

            context.Users.Add(admin);

            Post examplePost = new Post
                                   {
                                       Title = "Example post. You blog is now installed",
                                       Slug = "example",
                                       Content = "Example post",
                                       Author = admin,
                                       IsPublished = true,
                                       CreationDate = utcNow,
                                       LastModificationDate = utcNow,
                                       PublicationDate = utcNow,

                                   };

            context.Posts.Add(examplePost);

            base.Seed(context);
        }
    }
}
