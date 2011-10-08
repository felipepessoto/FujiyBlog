using System;
using System.Collections.Generic;
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

        public UserController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ViewResult Index()
        {
            return View(db.Users.ToList());
        }

        public virtual ActionResult Create()
        {
            if (!User.IsInRole(Role.CreateNewUsers))
            {
                Response.SendToUnauthorized();
            }

            return View(new AdminUserCreate());
        }

        [HttpPost]
        public virtual ActionResult Create(AdminUserCreate userData)
        {
            if (!User.IsInRole(Role.CreateNewUsers))
            {
                Response.SendToUnauthorized();
            }

            if (ModelState.IsValid)
            {
                User newUser = Mapper.Map<AdminUserCreate, User>(userData);
                newUser.Enabled = true;
                db.Users.Add(newUser);
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());  
            }

            return View(userData);
        }

        public virtual ActionResult Edit(int id)
        {
            User user = db.Users.Find(id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            return View(user);
        }

        [HttpPost]
        public virtual ActionResult Edit(AdminUserSave userData)
        {
            User user = db.Users.Single(x => x.Id == userData.Id);

            if (!(user.Username != User.Identity.Name && User.IsInRole(Role.EditOtherUsers)) &&
                    !(user.Username == User.Identity.Name && User.IsInRole(Role.EditOwnUser)))
            {
                Response.SendToUnauthorized();
            }

            if (ModelState.IsValid)
            {    
                Mapper.Map(userData, user);
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());
            }
            return View(user);
        }

        [HttpPost]
        public virtual ActionResult Disable(int id)
        {
            if (!db.Users.Any(x => x.Enabled && x.Id != id && x.RoleGroups.Any(y => y.Name == "Admin")))
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

            if (string.Equals(roleGroup.Name, "Admin", StringComparison.OrdinalIgnoreCase))
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

            if (string.Equals(roleGroup.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot edit admin roles");
            }

            if (!string.Equals(roleGroup.Name, "Anonymous", StringComparison.OrdinalIgnoreCase))
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

            if (string.Equals(deletedRoleGorup.Name, "Admin", StringComparison.OrdinalIgnoreCase) || string.Equals(deletedRoleGorup.Name, "Anonymous", StringComparison.OrdinalIgnoreCase))
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