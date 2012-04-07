using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Tasks;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Models;
using Microsoft.Web.Helpers;

namespace FujiyBlog.Web.Controllers
{
    public partial class CommentController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public CommentController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        [AuthorizeRole(Role.CreateComments)]
        public virtual ActionResult DoComment(int id, int? parentCommentId)
        {
            if (Settings.SettingRepository.ReCaptchaEnabled && !User.IsInRole(Role.ModerateComments) && !ReCaptcha.Validate(Settings.SettingRepository.ReCaptchaPrivateKey))
            {
                return Json(new { errorMessage = "Invalid Captcha!" });
            }

            bool isLogged = Request.IsAuthenticated;
            Post post = db.Posts.Include(x => x.Author).WhereHaveRoles().SingleOrDefault(x => x.Id == id);

            if (post == null || !post.IsCommentEnabled || !Settings.SettingRepository.EnableComments)
            {
                throw new InvalidOperationException();
            }

            if (Settings.SettingRepository.CloseCommentsAfterDays.HasValue && post.PublicationDate.AddDays(Settings.SettingRepository.CloseCommentsAfterDays.Value) < DateTime.UtcNow)
            {
                throw new InvalidOperationException();
            }

            PostComment postComment = new PostComment
            {
                CreationDate = DateTime.UtcNow,
                IpAddress = Request.UserHostAddress,
                Post = post,
                IsApproved = isLogged || !Settings.SettingRepository.ModerateComments,
            };

            if (isLogged)
            {
                postComment.Author = db.Users.SingleOrDefault(x => x.Username == User.Identity.Name);
                postComment.IsApproved = true;
                UpdateModel(postComment, new[] { "Comment" });
            }
            else
            {
                SocialUserData socialUserData = SocialController.GetLoggedUser();
                if (socialUserData != null)
                {
                    postComment.AuthorName = socialUserData.Name;
                    postComment.AuthorEmail = socialUserData.Email;
                    postComment.AuthorWebsite = socialUserData.WebSite;
                    UpdateModel(postComment, new[] { "Comment" });
                }
                else
                {
                    UpdateModel(postComment, new[] {"AuthorName", "AuthorEmail", "AuthorWebsite", "Comment"});
                }
            }

            if (parentCommentId.HasValue)
            {
                postComment.ParentComment = db.PostComments.Single(x => x.Id == parentCommentId.Value);
            }

            db.PostComments.Add(postComment);
            db.SaveChanges();

            if (!isLogged && Settings.SettingRepository.NotifyNewComments)
            {
                string subject = string.Format("Comment on \"{0}\" from {1}", post.Title, Settings.SettingRepository.BlogName);
                dynamic viewModel = new ExpandoObject();
                viewModel.BlogName = Settings.SettingRepository.BlogName;
                viewModel.Post = post;
                viewModel.Comment = postComment;
                string body = RenderPartialViewToString("NewComment", viewModel);

                new SendEmailTask(Settings.SettingRepository.EmailTo, subject, body).ExcuteLater();
            }

            return View("Comments", new[] { postComment });
        }

        [AuthorizeRole(Role.ModerateComments), HttpPost]
        public virtual ActionResult Approve(int id)
        {
            return ChangeCommentStatus(id, true);
        }

        [AuthorizeRole(Role.ModerateComments), HttpPost]
        public virtual ActionResult Disapprove(int id)
        {
            return ChangeCommentStatus(id, false);
        }

        [AuthorizeRole(Role.ModerateComments), HttpPost]
        public virtual ActionResult Delete(int id, bool deleteReplies)
        {
            PostComment deletedComment = db.PostComments.Single(x => x.Id == id);
            User moderatedBy = db.Users.Single(x => x.Username == User.Identity.Name);

            List<PostComment> commentsToDelete = new List<PostComment>();
            commentsToDelete.Add(deletedComment);

            if (deleteReplies)
            {
                commentsToDelete.AddRange(ReturnAllChildren(deletedComment));
            }

            foreach (PostComment comment in commentsToDelete.Where(x => !x.IsDeleted))
            {
                comment.IsDeleted = true;
                comment.ModeratedBy = moderatedBy;
            }

            db.SaveChanges(bypassValidation: true);

            return Json(true);
        }

        private IEnumerable<PostComment> ReturnAllChildren(PostComment comment)
        {
            IEnumerable<PostComment> replies = db.PostComments.Where(x => x.ParentComment.Id == comment.Id).ToList();
            return replies.Concat(replies.SelectMany(ReturnAllChildren));
        }

        private ActionResult ChangeCommentStatus(int id, bool approved)
        {
            PostComment comment = db.PostComments.Single(x => x.Id == id);
            comment.IsApproved = approved;
            comment.ModeratedBy = db.Users.Single(x => x.Username == User.Identity.Name);
            db.SaveChangesBypassingValidation();

            return Json(true);
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
