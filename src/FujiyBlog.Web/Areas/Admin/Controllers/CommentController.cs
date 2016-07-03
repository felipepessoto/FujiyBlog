using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Authorize(nameof(PermissionClaims.ModerateComments))]
    public class CommentController : AdminController
    {
        private readonly FujiyBlogDatabase db;

        public CommentController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public ViewResult Index(int? page)
        {
            AdminCommentIndex model = GetCommentsViewModel(page, false);

            return View(model);
        }

        public ViewResult Approved(int? page)
        {
            AdminCommentIndex model = GetCommentsViewModel(page, true);

            return View("Index", model);
        }

        [HttpPost]
        public virtual ActionResult ApproveSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();
            var moderatedBy = db.Users.Single(x => x.UserName == User.Identity.Name);

            foreach (PostComment postComment in comments)
            {
                postComment.ModeratedBy = moderatedBy;
                postComment.IsApproved = true;
            }
            
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DisapproveSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();
            var moderatedBy = db.Users.Single(x => x.UserName == User.Identity.Name);

            foreach (PostComment postComment in comments)
            {
                postComment.ModeratedBy = moderatedBy;
                postComment.IsApproved = false;
            }
            
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(IEnumerable<int> selectedComments)
        {
            List<PostComment> comments = db.PostComments.Where(x => selectedComments.Contains(x.Id)).ToList();
            var moderatedBy = db.Users.Single(x => x.UserName == User.Identity.Name);

            foreach (PostComment postComment in comments)
            {
                postComment.ModeratedBy = moderatedBy;
                postComment.IsDeleted = true;
            }
            
            db.SaveChanges();

            return Json(true);
        }

        private AdminCommentIndex GetCommentsViewModel(int? page, bool isApproved)
        {
            IQueryable<PostComment> comments = db.PostComments.Include(x => x.Author).Include(x => x.Post).Where(x => !x.IsDeleted && x.IsApproved == isApproved);

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
            db.PostComments.Single(x => x.Id == id).IsDeleted = true;
            db.SaveChanges();
            return Json(true);
        }
    }
}