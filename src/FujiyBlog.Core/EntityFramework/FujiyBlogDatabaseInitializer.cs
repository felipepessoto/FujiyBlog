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
            SeedDatabase(context);
            base.Seed(context);
        }

        public static void SeedDatabase(FujiyBlogDatabase context)
        {
            DateTime utcNow = DateTime.UtcNow;

            User admin = new User
                             {
                                 CreationDate = utcNow,
                                 Username = "admin",
                                 Password = "admin",
                                 Email = "admin@example.com",
                                 Enabled = true,
                             };

            context.Users.Add(admin);

            RoleGroup adminGroup = new RoleGroup {Name = "Admin"};
            adminGroup.Roles = Enum.GetValues(typeof (Role)).Cast<Role>();
            adminGroup.Users.Add(admin);

            RoleGroup editorGroup = new RoleGroup {Name = "Editor"};
            List<Role> editorRoles = new List<Role>
                                         {
                                             Role.AccessAdminPages,
                                             Role.ViewPublicPosts,
                                             Role.ViewUnpublishedPosts,
                                             Role.CreateNewPosts,
                                             Role.EditOwnPosts,
                                             Role.DeleteOwnPosts,
                                             Role.PublishOwnPosts,
                                             Role.EditOwnUser,
                                             Role.ViewPublicComments,
                                             Role.ViewUnmoderatedComments,
                                             Role.CreateComments,
                                             Role.ModerateComments,
                                             Role.ViewPublicPages,
                                             Role.ViewUnpublishedPages,
                                             Role.CreateNewPages,
                                             Role.EditOwnPages,
                                             Role.DeleteOwnPages,
                                             Role.PublishOwnPages,
                                         };

            editorGroup.Roles = editorRoles;

            RoleGroup anonymGroup = new RoleGroup {Name = "Anonymous"};
            List<Role> anonymRoles = new List<Role>
                                         {
                                             Role.ViewPublicPosts,
                                             Role.ViewPublicComments,
                                             Role.CreateComments,
                                             Role.ViewPublicPages,
                                         };

            anonymGroup.Roles = anonymRoles;

            context.RoleGroups.Add(adminGroup);
            context.RoleGroups.Add(editorGroup);
            context.RoleGroups.Add(anonymGroup);

            Post examplePost = new Post
                                   {
                                       Title = "Example post. You blog is now installed",
                                       Slug = "example",
                                       Content = "Example post",
                                       Author = admin,
                                       IsPublished = true,
                                       IsCommentEnabled = true,
                                       CreationDate = utcNow,
                                       LastModificationDate = utcNow,
                                       PublicationDate = utcNow,
                                   };

            context.Posts.Add(examplePost);
        }
    }
}
