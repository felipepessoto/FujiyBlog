using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;
using FujiyBlog.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class CommentController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public CommentController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ViewResult Index(int? page)
        {
            IQueryable<PostComment> comments = db.PostComments.Where(x => !x.IsDeleted && x.IsApproved);

            List<PostComment> pageComments =  comments.OrderByDescending(x=>x.CreationDate).Paging(page.GetValueOrDefault(1), 10).ToList();

            AdminCommentIndex model = new AdminCommentIndex
                                          {
                                              CurrentPage = page.GetValueOrDefault(1),
                                              Comments = pageComments,
                                              TotalPages = (int) Math.Ceiling(comments.Count()/(double) 10),
                                          };

            return View(model);
        }

        public virtual ViewResult Pending(int? page)
        {
            IQueryable<PostComment> comments = db.PostComments.Where(x => !x.IsDeleted && !x.IsApproved);

            List<PostComment> pageComments = comments.OrderByDescending(x => x.CreationDate).Paging(page.GetValueOrDefault(1), 10).ToList();

            AdminCommentIndex model = new AdminCommentIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                Comments = pageComments,
                TotalPages = (int)Math.Ceiling(comments.Count() / (double)10),
            };

            return View(MVC.Admin.Comment.Views.Index, model);
        }

        public virtual ActionResult Edit(int id)
        {
            PostComment postcomment = db.PostComments.Find(id);
            return View(postcomment);
        }

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

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            db.Database.ExecuteSqlCommand("UPDATE [PostComments] SET IsDeleted = 1 WHERE Id = {0}", id);
            return Json(true);
        }
    }
}