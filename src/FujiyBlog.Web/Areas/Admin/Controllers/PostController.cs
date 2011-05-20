using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using FujiyBlog.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class PostController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly FujiyBlogDatabase db;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;

        public PostController(IUnitOfWork unitOfWork, FujiyBlogDatabase db, IPostRepository postRepository, IUserRepository userRepository)
        {
            this.unitOfWork = unitOfWork;
            this.db = db;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }

        public virtual ViewResult Index(int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            AdminPostIndex model = new AdminPostIndex 
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, isPublic: false),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(isPublic: false) / (double)Settings.SettingRepository.PostsPerPage),
            };

            return View(model);
        }

        public virtual ActionResult Create()
        {
            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Post = new Post
                               {
                                   PublicationDate = DateTime.UtcNow.AddHours(Settings.SettingRepository.UtcOffset),
                                   IsPublished = true,
                                   IsCommentEnabled = true,
                               };
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTags = db.Tags.ToList();

            return View(viewModel);
        }

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

        public virtual ActionResult Edit(int id)
        {
            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Post = db.Posts.Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == id);
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTags = db.Tags.ToList();

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public virtual ActionResult EditPost(int id, string tags, IEnumerable<int> selectedCategories)
        {
            Post editedPost = db.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == id);

            TryUpdateModel(editedPost, new[] { "Title", "Description", "Slug", "Content", "PublicationDate", "IsPublished", "IsCommentEnabled" });

            if (db.Posts.Any(x => x.Slug == editedPost.Slug && x.Id != editedPost.Id))
            {
                ModelState.AddModelError("Slug", "This slug already exists");
            }

            if (ModelState.IsValid)
            {
                editedPost.Tags.Clear();
                foreach (Tag tag in postRepository.GetOrCreateTags(tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())))
                {
                    editedPost.Tags.Add(tag);
                }

                editedPost.Categories.Clear();

                foreach (Category category in db.Categories.Where(x => selectedCategories.Contains(x.Id)))
                {
                    editedPost.Categories.Add(category);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(editedPost);
        }

        public virtual ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            postRepository.DeletePost(id);
            unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public virtual ActionResult AddCategory(string name)
        {
            if (!db.Categories.Any(x => x.Name == name))
            {
                Category newCategory = db.Categories.Add(new Category {Name = name});
                unitOfWork.SaveChanges();
                return Json(newCategory);
            }
            return Json(false);
        }
    }
}