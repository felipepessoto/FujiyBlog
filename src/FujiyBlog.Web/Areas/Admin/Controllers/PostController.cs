using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Web.Areas.Admin.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PostController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private readonly PostRepository postRepository;
        private readonly IUserRepository userRepository;
        private const int PageSize = 10;

        public PostController(FujiyBlogDatabase db, PostRepository postRepository, IUserRepository userRepository)
        {
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
                .Paging(page.GetValueOrDefault(1), PageSize);

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
                TotalPages = (int)Math.Ceiling((double)posts.Count() / PageSize),
            };

            return View(model);
        }

        public virtual ActionResult Edit(int? id)
        {
            AdminPostEdit viewModel = new AdminPostEdit();
            Post post = id.HasValue ? db.Posts.Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == id)
                            : new Post
                                  {
                                      PublicationDate = DateTime.UtcNow,
                                      IsPublished = true,
                                      IsCommentEnabled = true
                                  };

            viewModel.Post = Mapper.Map<Post, AdminPostSave>(post);
            viewModel.Post.Id = id;
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
                                        });

            editedPost.LastModificationDate = DateTime.UtcNow;
            Mapper.Map(postSave, editedPost);

            if (db.Posts.Any(x => x.Slug == editedPost.Slug && x.Id != editedPost.Id))
            {
                ModelState.AddModelError("Post.Slug", "This slug already exists");
            }

            editedPost.Tags.Clear();
            if (postSave.Tags != null)
            {
                foreach (Tag tag in postRepository.GetOrCreateTags(postSave.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())))
                {
                    editedPost.Tags.Add(tag);
                }
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
            viewModel.Post = Mapper.Map<Post, AdminPostSave>(editedPost);
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
            db.SaveChanges();
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
            db.SaveChanges();
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
            db.SaveChanges();
            return Json(newCategory);
        }

        [HttpPost]
        public virtual ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Single(x => x.Id == id);
            db.Categories.Remove(category);
            db.SaveChanges();
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
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult DeleteTag(int id)
        {
            Tag tag = db.Tags.Single(x => x.Id == id);
            db.Tags.Remove(tag);
            db.SaveChanges();
            return Json(true);
        }
    }
}