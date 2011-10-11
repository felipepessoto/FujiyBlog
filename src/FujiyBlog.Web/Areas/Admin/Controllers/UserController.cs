using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Extensions;
using FujiyBlog.Web.Infrastructure;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class UserController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private const string AnonymousGroup = "Anonymous";
        private const string AdminGroup = "Admin";

        public UserController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ViewResult Index()
        {
            return View(db.Users.ToList());
        }

        [AuthorizeRole(Role.CreateNewUsers)]
        public virtual ActionResult Create()
        {
            AdminUserCreate viewModel = new AdminUserCreate();
            viewModel.SelectedRoleGroups = Enumerable.Empty<int>();
            viewModel.AllRoleGroups = db.RoleGroups.Where(x => x.Name != AnonymousGroup).ToList();

            return View(viewModel);
        }

        [HttpPost, AuthorizeRole(Role.CreateNewUsers)]
        public virtual ActionResult Create(AdminUserCreate userData)
        {
            if (db.Users.Any(x => x.Username == userData.Username && x.Id != userData.Id))
            {
                ModelState.AddModelError("Username", "This Username already exists");
            }

            if (ModelState.IsValid)
            {
                User newUser = Mapper.Map<AdminUserCreate, User>(userData);

                CheckEditRolesPermission(userData, newUser);

                newUser.Enabled = true;
                if (userData.SelectedRoleGroups != null && userData.SelectedRoleGroups.Count() > 0)
                {
                    newUser.RoleGroups = db.RoleGroups.Where(x => userData.SelectedRoleGroups.Any(y => x.Id == y)).ToList();
                }
                db.Users.Add(newUser);
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());  
            }

            userData.AllRoleGroups = db.RoleGroups.Where(x => x.Name != AnonymousGroup).ToList();
            return View(userData);
        }

        public virtual ActionResult Edit(int id)
        {
            User user = db.Users.Include(x => x.RoleGroups).Single(x => x.Id == id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            AdminUserSave viewModel = Mapper.Map<User, AdminUserSave>(user);
            viewModel.SelectedRoleGroups = user.RoleGroups.Select(x => x.Id);
            viewModel.AllRoleGroups = db.RoleGroups.Where(x => x.Name != AnonymousGroup).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Edit(AdminUserSave userData)
        {
            if (db.Users.Any(x => x.Username == userData.Username && x.Id != userData.Id))
            {
                ModelState.AddModelError("Username", "This Username already exists");
            }

            User user = db.Users.Include(x => x.RoleGroups).Single(x => x.Id == userData.Id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            CheckEditRolesPermission(userData, user);

            if (ModelState.IsValid)
            {    
                Mapper.Map(userData, user);
                user.RoleGroups = null;
                if (userData.SelectedRoleGroups != null && userData.SelectedRoleGroups.Count() > 0)
                {
                    user.RoleGroups = db.RoleGroups.Where(x => userData.SelectedRoleGroups.Any(y => x.Id == y)).ToList();
                }
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());
            }

            userData.AllRoleGroups = db.RoleGroups.Where(x => x.Name != AnonymousGroup).ToList();

            return View(userData);
        }

        private void CheckEditRolesPermission(AdminUserSave userData, User user)
        {
            var currentRoles = user.RoleGroups.Select(x => x.Id);
            var newRoles = userData.SelectedRoleGroups ?? Enumerable.Empty<int>();
            //If changed some role
            if (currentRoles.Except(newRoles).Count() != 0 || newRoles.Except(currentRoles).Count() != 0)
            {
                if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsersRoleGroups)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnRoleGroups)))
                {
                    Response.SendToUnauthorized();
                }
            }
        }

        [HttpPost]
        public virtual ActionResult Disable(int id)
        {
            if (!db.Users.Any(x => x.Enabled && x.Id != id && x.RoleGroups.Any(y => y.Name == AdminGroup)))
            {
                return Json(new { errorMessage = "You can´t disable the unique enabled admin" });
            }
            User user = db.Users.Find(id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            user.Enabled = false;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult Enable(int id)
        {
            User user = db.Users.Find(id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            user.Enabled = true;
            db.SaveChanges();
            return Json(true);
        }

        [AuthorizeRole(Role.ViewRoleGroups)]
        public virtual ViewResult RoleGroups()
        {
            return View(db.RoleGroups.ToList());
        }

        public virtual ActionResult EditRoleGroup(int? id)
        {
            RoleGroup roleGroup = id.HasValue ? db.RoleGroups.Find(id) : new RoleGroup();

            if (!id.HasValue && !User.IsInRole(Role.CreateRoleGroups))
            {
                Response.SendToUnauthorized();
            }

            if (id.HasValue && !User.IsInRole(Role.EditRoleGroups))
            {
                Response.SendToUnauthorized();
            }

            if (string.Equals(roleGroup.Name, AdminGroup, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot edit admin roles");
            }

            return View(roleGroup);
        }

        [HttpPost]
        public virtual ActionResult EditRoleGroup(int? id, string name, IEnumerable<string> roles)
        {
            RoleGroup roleGroup = id.HasValue ? db.RoleGroups.Find(id) : db.RoleGroups.Add(new RoleGroup());
            roles = roles ?? Enumerable.Empty<string>();

            if (!id.HasValue && !User.IsInRole(Role.CreateRoleGroups))
            {
                Response.SendToUnauthorized();
            }

            if (id.HasValue && !User.IsInRole(Role.EditRoleGroups))
            {
                Response.SendToUnauthorized();
            }

            if (string.Equals(roleGroup.Name, AdminGroup, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot edit admin roles");
            }

            if (!string.Equals(roleGroup.Name, AnonymousGroup, StringComparison.OrdinalIgnoreCase))
            {
                roleGroup.Name = name;
            }

            if (db.RoleGroups.Any(x => x.Name == roleGroup.Name && x.Id != id))
            {
                ModelState.AddModelError("Name", "This name already exists");
            }

            if (ModelState.IsValid)
            {
                roleGroup.Roles = roles.Select(x => (Role) Enum.Parse(typeof (Role), x));
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.RoleGroups());
            }

            return View(roleGroup);
        }

        [HttpPost]
        public virtual ActionResult DeleteRoleGroup(int id)
        {
            RoleGroup deletedRoleGorup = db.RoleGroups.Single(x => x.Id == id);

            if (string.Equals(deletedRoleGorup.Name, AdminGroup, StringComparison.OrdinalIgnoreCase) || string.Equals(deletedRoleGorup.Name, AnonymousGroup, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot delete " + deletedRoleGorup.Name + " roles");
            }

            if (!User.IsInRole(Role.DeleteRoleGroups))
            {
                Response.SendToUnauthorized();
            }

            db.RoleGroups.Remove(deletedRoleGorup);
            db.SaveChanges();

            return Json(true);
        }
    }
}