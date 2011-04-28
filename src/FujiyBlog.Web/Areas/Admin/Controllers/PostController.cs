using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;
using FujiyBlog.EntityFramework;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class PostController : Controller
    {
        private readonly FujiyBlogDatabase db;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;

        public PostController(FujiyBlogDatabase db, IPostRepository postRepository, IUserRepository userRepository)
        {
            this.db = db;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }

        //
        // GET: /Admin/Post/

        public virtual ViewResult Index(int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, isPublic: false),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(isPublic: false) / (double)Settings.SettingRepository.PostsPerPage),
            };

            return View(model);
        }

        //
        // GET: /Admin/Post/Create

        public virtual ActionResult Create()
        {
            Post newPost = new Post
                               {
                                   PublicationDate = DateTime.UtcNow.AddHours(Settings.SettingRepository.UtcOffset),
                                   IsPublished = true,
                                   IsCommentEnabled = true,
                               };
            return View(newPost);
        }

        //
        // POST: /Admin/Post/Create

        [HttpPost, ActionName("Create")]
        public virtual ActionResult CreatePost()
        {
            Post newPost = new Post
            {
                Author = userRepository.GetByUsername(User.Identity.Name),
                CreationDate = DateTime.UtcNow,
                LastModificationDate= DateTime.UtcNow,
            };

            TryUpdateModel(newPost, new[] { "Title","Description","Slug","Content","PublicationDate","IsPublished","IsCommentEnabled"});

            if (db.Posts.Any(x => x.Slug == newPost.Slug))
            {
                ModelState.AddModelError("Slug", "This slug already exists");
            }

            if (ModelState.IsValid)
            {
                db.Posts.Add(newPost);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(newPost);
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