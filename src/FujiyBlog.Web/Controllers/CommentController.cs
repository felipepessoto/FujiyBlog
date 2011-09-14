using System;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Models;
using System.Data.Entity;

namespace FujiyBlog.Web.Controllers
{
    public partial class CommentController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public CommentController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ActionResult DoComment(int id, int? parentCommentId)
        {
            bool isLogged = Request.IsAuthenticated;
            Post post = GetPost(id, !isLogged);

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
                UpdateModel(postComment, new[] { "AuthorName", "AuthorEmail", "AuthorWebsite", "Comment" });
            }

            if (parentCommentId.HasValue)
            {
                postComment.ParentComment = db.PostComments.Single(x => x.Id == parentCommentId.Value);
            }

            db.PostComments.Add(postComment);
            db.SaveChanges();

            return View("Comments", new[] { postComment });
        }

        public virtual ActionResult ReplyComment(int id)
        {
            PostComment comment = db.PostComments.Include(x => x.Post).Single(x => x.Id == id);

            PostComment newComment = new PostComment {Post = comment.Post, ParentComment = comment};

            return View(MVC.Themes.Views.Default.Comment.DoComment, newComment);
        }

        private Post GetPost(int id, bool isPublic)
        {
            IQueryable<Post> posts = db.Posts.Include(x => x.Author).Where(x => !x.IsDeleted);

            if (isPublic)
            {
                posts = posts.WhereIsPublicPost();
            }

            Post post = posts.SingleOrDefault(x => x.Id == id);

            if (post == null)
            {
                return null;
            }
            return post;
        }
    }
}
