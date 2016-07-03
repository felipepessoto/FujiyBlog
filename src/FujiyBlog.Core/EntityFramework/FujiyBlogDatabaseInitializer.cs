using System;
using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace FujiyBlog.Core.EntityFramework
{
    public class FujiyBlogDatabaseInitializer// : CreateDatabaseIfNotExists<FujiyBlogDatabase>
    {
        //protected override void Seed(FujiyBlogDatabase context)
        //{
        //    SeedDatabase(context);
        //    base.Seed(context);
        //}

        public static async Task SeedDatabase(FujiyBlogDatabase context, RoleManager<IdentityRole> _roleManager)
        {
            DateTime utcNow = DateTime.UtcNow;

            //User admin = new User
            //{
            //    CreationDate = utcNow,
            //    Username = "admin",
            //    Password = "admin",
            //    Email = "admin@example.com",
            //    Enabled = true,
            //};

            //context.Users.Add(admin);

            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                await _roleManager.CreateAsync(adminRole);

                foreach (var role in Enum.GetNames(typeof(PermissionClaims)))
                {
                    await _roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, role));
                }
            }

            List<PermissionClaims> editorRoles = new List<PermissionClaims>
                                         {
                                             PermissionClaims.AccessAdminPages,
                                             PermissionClaims.ViewPublicPosts,
                                             PermissionClaims.ViewUnpublishedPosts,
                                             PermissionClaims.CreateNewPosts,
                                             PermissionClaims.EditOwnPosts,
                                             PermissionClaims.DeleteOwnPosts,
                                             PermissionClaims.PublishOwnPosts,
                                             PermissionClaims.EditOwnUser,
                                             PermissionClaims.ViewPublicComments,
                                             PermissionClaims.ViewUnmoderatedComments,
                                             PermissionClaims.CreateComments,
                                             PermissionClaims.ModerateComments,
                                             PermissionClaims.ViewPublicPages,
                                             PermissionClaims.ViewUnpublishedPages,
                                             PermissionClaims.CreateNewPages,
                                             PermissionClaims.EditOwnPages,
                                             PermissionClaims.DeleteOwnPages,
                                             PermissionClaims.PublishOwnPages,
                                         };

            var editorRole = await _roleManager.FindByNameAsync("Editor");
            if (editorRole == null)
            {
                editorRole = new IdentityRole("Editor");
                await _roleManager.CreateAsync(editorRole);

                foreach (var role in editorRoles)
                {
                    await _roleManager.AddClaimAsync(editorRole, new Claim(CustomClaimTypes.Permission, role.ToString()));
                }
            }

            List<PermissionClaims> anonymRoles = new List<PermissionClaims>
                                         {
                                             PermissionClaims.ViewPublicPosts,
                                             PermissionClaims.ViewPublicComments,
                                             PermissionClaims.CreateComments,
                                             PermissionClaims.ViewPublicPages,
                                         };

            var anonymousRole = await _roleManager.FindByNameAsync("Anonymous");
            if (anonymousRole == null)
            {
                anonymousRole = new IdentityRole("Anonymous");
                await _roleManager.CreateAsync(anonymousRole);

                foreach (var role in anonymRoles)
                {
                    await _roleManager.AddClaimAsync(anonymousRole, new Claim(CustomClaimTypes.Permission, role.ToString()));
                }
            }

            //Post examplePost = new Post
            //{
            //    Title = "Example post. You blog is now installed",
            //    Slug = "example",
            //    Content = "Example post",
            //    Author = admin,
            //    IsPublished = true,
            //    IsCommentEnabled = true,
            //    CreationDate = utcNow,
            //    LastModificationDate = utcNow,
            //    PublicationDate = utcNow,
            //};

            //context.Posts.Add(examplePost);
        }
    }
}
