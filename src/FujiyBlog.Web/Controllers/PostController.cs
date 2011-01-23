using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Web.Controllers
{
    public partial class PostController : Controller
    {
        private readonly IPostRepository postRepository;

        public PostController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
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

        public virtual ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public virtual ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public virtual ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public virtual ActionResult Delete(int id)
        {
            return View();
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
