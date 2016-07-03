using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Controllers
{
    public partial class CommentController : Controller
    {
        private readonly FujiyBlogDatabase db;
        private readonly SettingRepository settings;
        private readonly ICompositeViewEngine viewEngine;

        public CommentController(FujiyBlogDatabase db, SettingRepository settings, ICompositeViewEngine viewEngine)
        {
            this.db = db;
            this.settings = settings;
            this.viewEngine = viewEngine;
        }

        [Authorize(nameof(PermissionClaims.CreateComments))]
        public async Task<ActionResult> DoComment(int id, int? parentCommentId)
        {
            if (settings.ReCaptchaEnabled && !HttpContext.UserHasClaimPermission(PermissionClaims.ModerateComments) && (await ValidateRecaptcha(Request, settings, Request.Form["g-recaptcha-response"]) == false))
            {
                return Json(new { errorMessage = "Invalid Captcha!" });
            }

            bool isLogged = User.Identity.IsAuthenticated;
            Post post = db.Posts.Include(x => x.Author).WhereHaveRoles(HttpContext).SingleOrDefault(x => x.Id == id);

            if (post == null || !post.IsCommentEnabled || !settings.EnableComments)
            {
                throw new InvalidOperationException();
            }

            if (settings.CloseCommentsAfterDays.HasValue && post.PublicationDate.AddDays(settings.CloseCommentsAfterDays.Value) < DateTime.UtcNow)
            {
                throw new InvalidOperationException();
            }

            PostComment postComment = new PostComment
            {
                CreationDate = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                Post = post,
                IsApproved = isLogged || !settings.ModerateComments,
            };

            if (isLogged)
            {
                postComment.Author = db.Users.SingleOrDefault(x => x.UserName == User.Identity.Name);
                postComment.IsApproved = true;

                await TryUpdateModelAsync(postComment, "", x => x.Comment);
            }
            else
            {
                SocialUserData socialUserData = null;//TODO SocialController.GetLoggedUser();
                if (socialUserData != null)
                {
                    postComment.AuthorName = socialUserData.Name;
                    postComment.AuthorEmail = socialUserData.Email;
                    postComment.AuthorWebsite = socialUserData.WebSite;
                    await TryUpdateModelAsync(postComment, "", x => x.Comment);
                }
                else
                {
                    await TryUpdateModelAsync(postComment, "", x => x.AuthorName, x => x.AuthorEmail, x => x.AuthorWebsite, x => x.Comment);
                }
            }

            if (parentCommentId.HasValue)
            {
                postComment.ParentComment = db.PostComments.Single(x => x.Id == parentCommentId.Value);
            }

            db.PostComments.Add(postComment);
            db.SaveChanges();

            if (!isLogged && settings.NotifyNewComments)
            {
                string subject = string.Format("Comment on \"{0}\" from {1}", post.Title, settings.BlogName);
                dynamic viewModel = new ExpandoObject();
                viewModel.BlogName = settings.BlogName;
                viewModel.Post = post;
                viewModel.Comment = postComment;
                string body = RenderPartialViewToString("NewComment", viewModel);

                await new EmailService(settings).Send(settings.EmailTo, subject, body, true, null, null);
            }

            return View("Comments", new[] { postComment });
        }

        //TODO Should Refactor to proper class
        internal static async Task<bool> ValidateRecaptcha(HttpRequest request, SettingRepository settings, string gRecaptchaResponse)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(new[]
               {
                new KeyValuePair<string, string>("secret", settings.ReCaptchaPrivateKey),
                new KeyValuePair<string, string>("response", gRecaptchaResponse),
                new KeyValuePair<string, string>("remoteip", request.HttpContext.Connection.RemoteIpAddress.ToString()),//.ServerVariables["REMOTE_ADDR"]
            }))
                {

                    using (var result = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content))
                    {
                        var resultContent = await result.Content.ReadAsStringAsync();
                        var resultJson = JObject.Parse(resultContent);
                        return (bool)resultJson["success"];
                    }
                }
            }
        }

        [Authorize(nameof(PermissionClaims.ModerateComments)), HttpPost]
        public ActionResult Approve(int id)
        {
            return ChangeCommentStatus(id, true);
        }

        [Authorize(nameof(PermissionClaims.ModerateComments)), HttpPost]
        public ActionResult Disapprove(int id)
        {
            return ChangeCommentStatus(id, false);
        }

        [Authorize(nameof(PermissionClaims.ModerateComments)), HttpPost]
        public ActionResult Delete(int id, bool deleteReplies)
        {
            PostComment deletedComment = db.PostComments.Single(x => x.Id == id);
            var moderatedBy = db.Users.Single(x => x.UserName == User.Identity.Name);

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

            db.SaveChanges();

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
            comment.ModeratedBy = db.Users.Single(x => x.UserName == User.Identity.Name);
            db.SaveChanges();

            return Json(true);
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
