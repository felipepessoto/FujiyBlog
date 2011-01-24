using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.Services;
using FujiyBlog.Core.Infrastructure;

namespace FujiyBlog.Web.Controllers
{
    public partial class PostController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPostRepository postRepository;
        private readonly PostService postService;

        public PostController(IUnitOfWork unitOfWork, IPostRepository postRepository, PostService postService)
        {
            this.unitOfWork = unitOfWork;
            this.postRepository = postRepository;
            this.postService = postService;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Details(string postSlug)
        {
            Post post = postRepository.GetPost(postSlug);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        public virtual ActionResult DetailsById(int id)
        {
            Post post = postRepository.GetPost(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(MVC.Post.Views.Details, post);
        }

        public virtual ActionResult DoComment(int id)
        {
            PostComment postComment = new PostComment();

            postComment.IpAddress = Request.UserHostAddress;
            postComment.Post = postRepository.GetPost(id);

            if (TryUpdateModel(postComment, new[] { "AuthorName", "AuthorEmail", "AuthorWebsite", "Comment" }))
            {
                postService.AddComment(postComment);
                unitOfWork.SaveChanges();

                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
