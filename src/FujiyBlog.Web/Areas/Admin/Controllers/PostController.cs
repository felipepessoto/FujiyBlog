using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using FujiyBlog.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PostController : AdminController
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

        public virtual ViewResult Index(int? page, bool? published)
        {
            IQueryable<Post> posts = db.Posts.Where(x => !x.IsDeleted);

            if (published.HasValue)
            {
                posts = published.Value ? posts.Where(x => x.IsPublished) : posts.Where(x => !x.IsPublished);
            }

            IQueryable<Post> pagePosts = posts.OrderByDescending(x => x.PublicationDate).Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories)
                .Paging(page.GetValueOrDefault(), Settings.SettingRepository.PostsPerPage);

            Dictionary<int, int> counts = (from post in pagePosts
                      select new { post.Id, C = post.Comments.Count() }).ToDictionary(e => e.Id, e => e.C);

            AdminPostIndex model = new AdminPostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = (from post in pagePosts.ToList()
                               select new PostSummary
                                          {
                                              Post = post,
                                              CommentsTotal = counts[post.Id]
                                          }),
                TotalPages = (int)Math.Ceiling(posts.Count() / (double)Settings.SettingRepository.PostsPerPage),
            };

            return View(model);
        }

        public virtual ActionResult Edit(int? id)
        {
            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Post = id.HasValue ? db.Posts.Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == id)
                                 : new Post
                                       {
                                           PublicationDate =
                                               DateTime.UtcNow.AddHours(Settings.SettingRepository.UtcOffset),
                                           IsPublished = true,
                                           IsCommentEnabled = true
                                       };
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTags = db.Tags.ToList();

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public virtual ActionResult EditPost([Bind(Prefix="Post")]AdminPostSave postSave)
        {
            Post editedPost = postSave.Id.HasValue ? db.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == postSave.Id)
                                  : db.Posts.Add(new Post
                                        {
                                            Author = userRepository.GetByUsername(User.Identity.Name),
                                            CreationDate = DateTime.UtcNow,
                                            LastModificationDate = DateTime.UtcNow
                                        });

            editedPost.Title = postSave.Title;
            editedPost.Description = postSave.Description;
            editedPost.Slug = postSave.Slug;
            editedPost.Content = postSave.Content;
            editedPost.PublicationDate = postSave.PublicationDate.AddHours(-Settings.SettingRepository.UtcOffset);
            editedPost.IsPublished = postSave.IsPublished;
            editedPost.IsCommentEnabled = postSave.IsCommentEnabled;

            //TryUpdateModel(editedPost, "Post", new[] { "Title", "Description", "Slug", "Content", "PublicationDate", "IsPublished", "IsCommentEnabled" });

            if (db.Posts.Any(x => x.Slug == editedPost.Slug && x.Id != editedPost.Id))
            {
                ModelState.AddModelError("Post.Slug", "This slug already exists");
            }

            editedPost.Tags.Clear();
            foreach (Tag tag in postRepository.GetOrCreateTags(postSave.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())))
            {
                editedPost.Tags.Add(tag);
            }

            editedPost.Categories.Clear();

            if (postSave.SelectedCategories != null)
            {
                foreach (Category category in db.Categories.Where(x => postSave.SelectedCategories.Contains(x.Id)))
                {
                    editedPost.Categories.Add(category);
                }
            }
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Post = editedPost;
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTags = db.Tags.ToList();

            return View(viewModel);
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
            if (db.Categories.Any(x => x.Name == name))
            {
                return Json(new { errorMessage = "The category already exist" });
            }

            Category newCategory = db.Categories.Add(new Category {Name = name});
            unitOfWork.SaveChanges();
            return Json(newCategory);
        }
    }
}