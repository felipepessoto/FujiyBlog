using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class UserController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private const string AnonymousRole = "Anonymous";
        private const string AdminRole = "Admin";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(FujiyBlogDatabase db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public ViewResult Index()
        {
            return View(db.Users.ToList());
        }

        [Authorize(nameof(PermissionClaims.CreateNewUsers))]
        public ActionResult Create()
        {
            AdminUserCreate viewModel = new AdminUserCreate();
            viewModel.AllRoles = db.Roles.Where(x => x.Name != AnonymousRole).ToList();

            return View(viewModel);
        }

        [HttpPost, Authorize(nameof(PermissionClaims.CreateNewUsers))]
        public async Task<ActionResult> Create(AdminUserCreate userData)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = userData.Username,
                    Email = userData.Email,
                    DisplayName = userData.DisplayName,
                    FullName = userData.FullName,
                    BirthDate = userData.BirthDate,
                    Location = userData.Location,
                    About = userData.About,
                    Enabled = true,
                    CreationDate = DateTime.UtcNow,
                };

                if (CheckEditRolesPermission(userData, user) == false)
                {
                    return Forbid();
                }

                var result = await userManager.CreateAsync(user, userData.Password);

                if (result.Succeeded)
                {
                    result = await userManager.AddToRolesAsync(user, userData.SelectedRoles);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                AddErrors(result);
            }

            userData.AllRoles = db.Roles.Where(x => x.Name != AnonymousRole).ToList();
            return View(userData);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public ActionResult Edit(string id)
        {
            var user = db.Users.Include(x => x.Roles).Single(x => x.Id == id);

            if (!(user.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsers)) &&
                    !(user.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnUser)))
            {
                return Forbid();
            }

            AdminUserSave viewModel = new AdminUserSave(user);
            viewModel.AllRoles = db.Roles.Where(x => x.Name != AnonymousRole).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AdminUserSave userData)
        {
            var user = db.Users.Include(x => x.Roles).Single(x => x.Id == userData.Id);

            if (!(user.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsers)) &&
                    !(user.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnUser)))
            {
                return Forbid();
            }

            if (CheckEditRolesPermission(userData, user) == false)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                user.UserName = userData.Username;
                user.Email = userData.Email;
                user.DisplayName = userData.DisplayName;
                user.FullName = userData.FullName;
                user.BirthDate = userData.BirthDate;
                user.Location = userData.Location;
                user.About = userData.About;
                user.Enabled = true;

                var rolesRemoved = new List<string>();
                foreach (var removedRole in user.Roles.Where(x => userData.SelectedRoles.Contains(x.RoleId) == false))
                {
                    var role = await roleManager.FindByIdAsync(removedRole.RoleId);
                    rolesRemoved.Add(role.Name);
                }

                var result = await userManager.RemoveFromRolesAsync(user, rolesRemoved);

                if (result.Succeeded)
                {
                    var rolesAdded = new List<string>();
                    foreach (var addedRole in userData.SelectedRoles.Where(x => user.Roles.Any(y => y.RoleId == x) == false))
                    {
                        var role = await roleManager.FindByIdAsync(addedRole);
                        rolesAdded.Add(role.Name);
                    }

                    result = await userManager.AddToRolesAsync(user, rolesAdded);

                    if (result.Succeeded)
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    AddErrors(result);
                }
            }

            userData.AllRoles = db.Roles.Where(x => x.Name != AnonymousRole).ToList();

            return View(userData);
        }

        private bool CheckEditRolesPermission(AdminUserSave userData, ApplicationUser user)
        {
            var currentRoles = user.Roles.Select(x => x.RoleId);
            var newRoles = userData.SelectedRoles ?? Enumerable.Empty<string>();
            //If changed some role
            if (currentRoles.Except(newRoles).Count() != 0 || newRoles.Except(currentRoles).Count() != 0)
            {
                if (!(user.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersRoles)) &&
                    !(user.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnRoles)))
                {
                    return false;
                }
            }

            return true;
        }

        [HttpPost]
        public ActionResult Disable(string id)
        {
            if (!db.Users.Any(x => x.Enabled && x.Id != id && x.Roles.Any(y => db.Roles.Any(z => z.Id == y.RoleId && z.Name == AdminRole))))
            {
                return Json(new { errorMessage = "You can´t disable the unique enabled admin" });
            }
            ApplicationUser user = db.Users.SingleOrDefault(x => x.Id == id);

            if (!(user.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsers)) &&
                    !(user.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnUser)))
            {
                return Forbid();
            }

            user.Enabled = false;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult Enable(string id)
        {
            var user = db.Users.SingleOrDefault(x => x.Id == id);

            if (!(user.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsers)) &&
                    !(user.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnUser)))
            {
                return Forbid();
            }

            user.Enabled = true;
            db.SaveChanges();
            return Json(true);
        }

        [Authorize(nameof(PermissionClaims.ViewRoles))]
        public ViewResult Roles()
        {
            return View(db.Roles.ToList());
        }

        public async Task<ActionResult> EditRole(string id)
        {
            IdentityRole role = id != null ? await roleManager.FindByIdAsync(id) : null;

            if (id == null && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateRoles))
            {
                return Forbid();
            }

            if (id != null && !HttpContext.UserHasClaimPermission(PermissionClaims.EditRoles))
            {
                return Forbid();
            }

            if (string.Equals(role?.Name, AdminRole, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot edit admin roles");
            }

            var model = new AdminRoleSave();
            model.Id = id;
            model.Name = role?.Name;
            model.Claims = role != null ? (await roleManager.GetClaimsAsync(role)).Where(x=>x.Type == CustomClaimTypes.Permission).Select(x=>x.Value) : Enumerable.Empty<string>();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(string id, string name, IEnumerable<string> claims)
        {
            var role = id != null ? await roleManager.FindByIdAsync(id) : db.Roles.Add(new IdentityRole()).Entity;
            claims = claims ?? Enumerable.Empty<string>();

            if (id == null && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateRoles))
            {
                return Forbid();
            }

            if (id != null && !HttpContext.UserHasClaimPermission(PermissionClaims.EditRoles))
            {
                return Forbid();
            }

            if (string.Equals(role.Name, AdminRole, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot edit admin roles");
            }

            if (!string.Equals(role.Name, AnonymousRole, StringComparison.OrdinalIgnoreCase))
            {
                role.Name = name;
            }

            if (ModelState.IsValid)
            {
                IdentityResult result = IdentityResult.Success;

                if (id == null)
                {
                    result = await roleManager.CreateAsync(role);
                }

                if (result.Succeeded)
                {
                    var currentClaims = await roleManager.GetClaimsAsync(role);

                    foreach (var removedClaim in currentClaims.Where(x => x.Type == CustomClaimTypes.Permission && claims.Contains(x.Value) == false).ToList())
                    {
                        result = await roleManager.RemoveClaimAsync(role, removedClaim);
                        if (result.Succeeded == false)
                        {
                            break;
                        }
                    }

                    if (result.Succeeded)
                    {

                        var rolesAdded = new List<string>();
                        foreach (var addedRole in claims.Except(currentClaims.Where(y => y.Type == CustomClaimTypes.Permission).Select(x => x.Value)).ToList())
                        {
                            result = await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, addedRole));

                            if (result.Succeeded == false)
                            {
                                break;
                            }
                        }

                        if (result.Succeeded)
                        {
                            db.SaveChanges();
                            return RedirectToAction("Roles");
                        }
                    }
                }

                AddErrors(result);
            }

            var model = new AdminRoleSave();
            model.Id = id;
            model.Name = role.Name;
            model.Claims = claims;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRole(string id)
        {
            IdentityRole deletedRoleGorup =await roleManager.FindByIdAsync(id);

            if (string.Equals(deletedRoleGorup.Name, AdminRole, StringComparison.OrdinalIgnoreCase) || string.Equals(deletedRoleGorup.Name, AnonymousRole, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot delete " + deletedRoleGorup.Name + " roles");
            }

            if (!HttpContext.UserHasClaimPermission(PermissionClaims.DeleteRoles))
            {
                return Forbid();
            }

            await roleManager.DeleteAsync(deletedRoleGorup);
            
            return Json(true);
        }
    }
}
