using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.Common;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using FujiyBlog.EntityFramework;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Models;

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
                .Paging(page.GetValueOrDefault(1), 10);

            Dictionary<int, int> counts = (from post in pagePosts
                      select new { post.Id, C = post.Comments.Where(x => !x.IsDeleted).Count() }).ToDictionary(e => e.Id, e => e.C);

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
                                           PublicationDate = DateTime.UtcNow,
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
            editedPost.PublicationDate = DateTimeUtil.ConvertMyTimeZoneToUtc(postSave.PublicationDate);
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

        public virtual ActionResult Categories()
        {
            Dictionary<Category, int> categoriesPostCount = (from category in db.Categories
                                                             select new { Category = category, PostCount = category.Posts.Where(x => !x.IsDeleted).Count() }).ToDictionary(e => e.Category, e => e.PostCount);

            AdminCategoriesList adminCategoriesList = new AdminCategoriesList { CategoriesPostCount = categoriesPostCount };
            return View(adminCategoriesList);
        }

        [HttpPost]
        public virtual ActionResult UpdateCategory(int id, string name)
        {
            if (db.Categories.Any(x => x.Id != id && x.Name == name))
            {
                return Json(new {errorMessage = "The category already exist"});
            }

            Category category = db.Categories.Single(x => x.Id == id);

            category.Name = name;
            unitOfWork.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult AddCategory([Bind(Include = "Name", Prefix = "NewCategory")]Category newCategory)
        {
            if (db.Categories.Any(x => x.Name == newCategory.Name))
            {
                return Json(new { errorMessage = "The category already exist" });
            }

            db.Categories.Add(newCategory);
            unitOfWork.SaveChanges();
            return Json(newCategory);
        }

        [HttpPost]
        public virtual ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Single(x => x.Id == id);
            db.Categories.Remove(category);
            unitOfWork.SaveChanges();
            return Json(true);
        }

        public virtual ActionResult Tags()
        {
            Dictionary<Tag, int> tagsPostCount = (from tag in db.Tags
                                                             select new { Tag = tag, PostCount = tag.Posts.Count() }).ToDictionary(e => e.Tag, e => e.PostCount);

            AdminTagsList tagsList = new AdminTagsList {TagsPostCount = tagsPostCount};
            return View(tagsList);
        }

        [HttpPost]
        public virtual ActionResult UpdateTag(int id, string name)
        {
            if (db.Tags.Any(x => x.Id != id && x.Name == name))
            {
                return Json(new { errorMessage = "The tag already exist" });
            }

            Tag tag = db.Tags.Single(x => x.Id == id);

            tag.Name = name;
            unitOfWork.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DeleteTag(int id)
        {
            Tag tag = db.Tags.Single(x => x.Id == id);
            db.Tags.Remove(tag);
            unitOfWork.SaveChanges();
            return Json(true);
        }
    }
}