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
    public partial class CommentController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public CommentController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        //
        // GET: /Admin/Comment/

        public virtual ViewResult Index()
        {
            return View(db.PostComments.ToList());
        }

        //
        // GET: /Admin/Comment/Details/5

        public virtual ViewResult Details(int id)
        {
            PostComment postcomment = db.PostComments.Find(id);
            return View(postcomment);
        }

        //
        // GET: /Admin/Comment/Create

        public virtual ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Comment/Create

        [HttpPost]
        public virtual ActionResult Create(PostComment postcomment)
        {
            if (ModelState.IsValid)
            {
                db.PostComments.Add(postcomment);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(postcomment);
        }
        
        //
        // GET: /Admin/Comment/Edit/5

        public virtual ActionResult Edit(int id)
        {
            PostComment postcomment = db.PostComments.Find(id);
            return View(postcomment);
        }

        //
        // POST: /Admin/Comment/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(PostComment postcomment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postcomment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postcomment);
        }

        //
        // GET: /Admin/Comment/Delete/5

        public virtual ActionResult Delete(int id)
        {
            PostComment postcomment = db.PostComments.Find(id);
            return View(postcomment);
        }

        //
        // POST: /Admin/Comment/Delete/5

        [HttpPost, ActionName("Delete")]
        public virtual ActionResult DeleteConfirmed(int id)
        {            
            PostComment postcomment = db.PostComments.Find(id);
            db.PostComments.Remove(postcomment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}