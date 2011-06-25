using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;

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
            return View(new AdminUserCreate());
        }

        [HttpPost]
        public virtual ActionResult Create(AdminUserCreate userData)
        {
            if (ModelState.IsValid)
            {
                User newUser = Mapper.Map<AdminUserCreate, User>(userData);
                db.Users.Add(newUser);
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());  
            }

            return View(userData);
        }

        public virtual ActionResult Edit(int id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        public virtual ActionResult Edit(AdminUserSave userData)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Single(x => x.Id == userData.Id);
                Mapper.Map(userData, user);
                db.SaveChanges();
                return RedirectToAction(MVC.Admin.User.Index());
            }
            return View(userData);
        }

        [HttpPost]
        public virtual ActionResult Disable(int id)
        {
            if (!db.Users.Any(x => x.Enabled && x.Id != id))
            {
                return Json(new { errorMessage = "You can´t disable the unique enabled user" });
            }
            User user = db.Users.Find(id);
            user.Enabled = false;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult Enable(int id)
        {
            User user = db.Users.Find(id);
            user.Enabled = true;
            db.SaveChanges();
            return Json(true);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}