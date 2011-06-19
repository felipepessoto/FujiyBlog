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
            AdminCommentIndex model = GetCommentsViewModel(page, true);

            return View(model);
        }

        public virtual ViewResult Pending(int? page)
        {
            AdminCommentIndex model = GetCommentsViewModel(page, false);

            return View(MVC.Admin.Comment.Views.Index, model);
        }

        [HttpPost]
        public virtual ActionResult ApproveSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();

            foreach (PostComment postComment in comments)
            {
                postComment.IsApproved = true;
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;

            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DisapproveSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();

            foreach (PostComment postComment in comments)
            {
                postComment.IsApproved = false;
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;

            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();

            foreach (PostComment postComment in comments)
            {
                postComment.IsDeleted = true;
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;

            return Json(true);
        }

        private AdminCommentIndex GetCommentsViewModel(int? page, bool isApproved)
        {
            IQueryable<PostComment> comments = db.PostComments.Include(x => x.Author).Where(x => !x.IsDeleted && x.IsApproved == isApproved);

            List<PostComment> pageComments = comments.OrderByDescending(x => x.CreationDate).Paging(page.GetValueOrDefault(1), 10).ToList();

            return new AdminCommentIndex
                       {
                           CurrentPage = page.GetValueOrDefault(1),
                           Comments = pageComments,
                           TotalPages = (int) Math.Ceiling(comments.Count()/(double) 10),
                       };
        }

        public virtual ActionResult Edit(int id)
        {
            PostComment postcomment = db.PostComments.Include(x => x.Author).Single(x => x.Id == id);
            return View(postcomment);
        }

        [HttpPost]
        public virtual ActionResult Edit(AdminCommentSave input)
        {
            PostComment editedComment = db.PostComments.Include(x => x.Author).Include(x => x.Post).Single(x => x.Id == input.Id);
            if (ModelState.IsValid)
            {
                if (editedComment.Author == null)
                {
                    editedComment.AuthorName = input.AuthorName;
                    editedComment.AuthorEmail = input.AuthorEmail;
                    editedComment.AuthorWebsite = input.AuthorWebsite;
                }
                editedComment.Comment = input.Comment;


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(editedComment);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            db.PostComments.Find(id).IsDeleted = true;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;
            return Json(true);
        }
    }
}