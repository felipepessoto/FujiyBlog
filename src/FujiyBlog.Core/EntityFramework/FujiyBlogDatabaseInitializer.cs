using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
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
                                        Value = "Central Standard Time"
                                    };



            Setting emailTo = new Setting
                                    {
                                        Id = 7,
                                        Description = "Email To",
                                        Value = "example@domain.com"
                                    };

            Setting emailSubjectPrefix = new Setting
                                             {
                                                 Id = 8,
                                                 Description = "Subject Prefix",
                                                 Value = "Blog"
                                             };

            Setting smtpAddress = new Setting
                                      {
                                          Id = 9,
                                          Description = "Smtp Address",
                                          Value = "smtp.domain.com"
                                      };

            Setting smtpPort = new Setting
                                   {
                                       Id = 10,
                                       Description = "Smtp Port",
                                       Value = "25"
                                   };

            Setting smtpUserName = new Setting
                                       {
                                           Id = 11,
                                           Description = "Smtp UserName",
                                           Value = "user"
                                       };

            Setting smtpPassword = new Setting
                                       {
                                           Id = 12,
                                           Description = "Smtp Password",
                                           Value = ""
                                       };

            Setting smtpSsl = new Setting
                                  {
                                      Id = 13,
                                      Description = "Smtp SSL",
                                      Value = "False"
                                  };



            Setting enableComments = new Setting
                                         {
                                             Id = 14,
                                             Description = "Enable Comments",
                                             Value = "True"
                                         };

            Setting moderateComments = new Setting
                                           {
                                               Id = 15,
                                               Description = "Moderate Comments",
                                               Value = "False"
                                           };

            Setting enableNestedComments = new Setting
                                               {
                                                   Id = 16,
                                                   Description = "Enable Nested Comments",
                                                   Value = "True"
                                               };

            Setting closeCommentsAfterDays = new Setting
                                                 {
                                                     Id = 17,
                                                     Description = "Close Comments After Days",
                                                     Value = null
                                                 };

            Setting commentsPerPage = new Setting
                                          {
                                              Id = 18,
                                              Description = "Comments Per Page",
                                              Value = "10"
                                          };

            Setting commentsAvatar = new Setting
                                             {
                                                 Id = 19,
                                                 Description = "Comments Avatar",
                                                 Value = null
                                             };

            Setting culture = new Setting
            {
                Id = 20,
                Description = "Blog Culture",
                Value = "auto"
            };

            context.Settings.Add(minRequiredPasswordLength);
            context.Settings.Add(postsPerPage);
            context.Settings.Add(blogName);
            context.Settings.Add(blogDescription);
            context.Settings.Add(theme);
            context.Settings.Add(utcOffset);

            context.Settings.Add(emailTo);
            context.Settings.Add(emailSubjectPrefix);
            context.Settings.Add(smtpAddress);
            context.Settings.Add(smtpPort);
            context.Settings.Add(smtpUserName);
            context.Settings.Add(smtpPassword);
            context.Settings.Add(smtpSsl);

            context.Settings.Add(enableComments);
            context.Settings.Add(moderateComments);
            context.Settings.Add(enableNestedComments);
            context.Settings.Add(closeCommentsAfterDays);
            context.Settings.Add(commentsPerPage);
            context.Settings.Add(commentsAvatar);

            context.Settings.Add(culture);

            User admin = new User
                             {
                                 CreationDate = utcNow,
                                 Username = "admin",
                                 Password = "admin",
                                 Email = "admin@example.com",
                                 Enabled = true,
                             };

            context.Users.Add(admin);

            PermissionGroup adminGroup = new PermissionGroup {Name = "Admin"};
            adminGroup.Permissions = Enum.GetValues(typeof (Permission)).Cast<Permission>();
            adminGroup.Users.Add(admin);

            PermissionGroup editorGroup = new PermissionGroup { Name = "Editor" };
            List<Permission> editorPermissions = new List<Permission>
                                                     {
                                                         Permission.AccessAdminPages,
                                                         Permission.ViewPublicPosts,
                                                         Permission.ViewUnpublishedPosts,
                                                         Permission.CreateNewPosts,
                                                         Permission.EditOwnPosts,
                                                         Permission.DeleteOwnPosts,
                                                         Permission.PublishOwnPosts,
                                                         Permission.EditOwnUser,
                                                         Permission.ViewPublicComments,
                                                         Permission.ViewUnmoderatedComments,
                                                         Permission.CreateComments,
                                                         Permission.ModerateComments,
                                                         Permission.ViewPublicPages,
                                                         Permission.ViewUnpublishedPages,
                                                         Permission.CreateNewPages,
                                                         Permission.EditOwnPages,
                                                         Permission.DeleteOwnPages,
                                                         Permission.PublishOwnPages,
                                                     };

            editorGroup.Permissions = editorPermissions;

            PermissionGroup anonymGroup = new PermissionGroup { Name = "Anonymous" };
            List<Permission> anonymPermissions = new List<Permission>
                                                     {
                                                         Permission.ViewPublicPosts,
                                                         Permission.ViewPublicComments,
                                                         Permission.CreateComments,
                                                         Permission.ViewPublicPages,
                                                     };

            anonymGroup.Permissions = anonymPermissions;

            context.PermissionGroups.Add(adminGroup);
            context.PermissionGroups.Add(editorGroup);
            context.PermissionGroups.Add(anonymGroup);

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
