using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.EntityFramework;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PostController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public PostController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        //
        // GET: /Admin/Post/

        public virtual ViewResult Index()
        {
            return View(db.Posts.ToList());
        }

        //
        // GET: /Admin/Post/Details/5

        public virtual ViewResult Details(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        //
        // GET: /Admin/Post/Create

        public virtual ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Post/Create

        [HttpPost]
        public virtual ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(post);
        }
        
        //
        // GET: /Admin/Post/Edit/5

        public virtual ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        //
        // POST: /Admin/Post/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        //
        // GET: /Admin/Post/Delete/5

        public virtual ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        //
        // POST: /Admin/Post/Delete/5

        [HttpPost, ActionName("Delete")]
        public virtual ActionResult DeleteConfirmed(int id)
        {            
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}