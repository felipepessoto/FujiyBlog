using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Extensions;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PostController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private readonly PostRepository postRepository;
        private readonly UserRepository userRepository;
        private const int PageSize = 10;

        public PostController(FujiyBlogDatabase db, PostRepository postRepository, UserRepository userRepository)
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
            if (!id.HasValue && !User.IsInRole(Role.CreateNewPosts))
            {
                Response.SendToUnauthorized();
            }

            Post post = id.HasValue ? db.Posts.Include(x => x.Tags).Include(x => x.Categories).Include(x => x.Author).Single(x => x.Id == id)
                            : new Post
                                  {
                                      PublicationDate = DateTime.UtcNow,
                                      IsCommentEnabled = true,
                                      IsPublished = true,
                                      Author = db.Users.Single(x => x.Username == User.Identity.Name),
                                  };

            if (id.HasValue && !User.IsInRole(Role.EditOtherUsersPosts) && !(post.Author.Username == User.Identity.Name && User.IsInRole(Role.EditOwnPosts)))
            {
                Response.SendToUnauthorized();
            }

            return View(CreateAdminPostEdit(post));
        }

        [HttpPost]
        public virtual ActionResult Edit([Bind(Prefix="Post")]AdminPostSave postSave)
        {
            Post editedPost = postSave.Id.HasValue ? db.Posts.Include(x => x.Author).Include(x => x.Tags).Include(x => x.Categories).Single(x => x.Id == postSave.Id)
                                  : db.Posts.Add(new Post
                                        {
                                            CreationDate = DateTime.UtcNow,
                                        });

            User newAuthor = userRepository.GetById(postSave.AuthorId.Value); //postSave.AuthorId.HasValue ? userRepository.GetById(postSave.AuthorId.Value) : userRepository.GetByUsername(User.Identity.Name);

            CheckPostsSaveRoles(postSave, editedPost, newAuthor);

            editedPost.Author = newAuthor;
            editedPost.LastModificationDate = DateTime.UtcNow;
            postSave.FillPost(editedPost);

            if (db.Posts.Any(x => x.Slug == editedPost.Slug && x.Id != editedPost.Id))
            {
                ModelState.AddModelError("Post.Slug", "This slug already exists");
            }

            editedPost.Tags.Clear();
            if (postSave.Tags != null)
            {
                IEnumerable<string> tags = from tag in postSave.Tags.Split(new[] {','})
                                           where !string.IsNullOrWhiteSpace(tag)
                                           select tag.Trim();

                foreach (Tag tag in postRepository.GetOrCreateTags(tags))
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
                CreatePostRevision(editedPost);
                return RedirectToAction(MVC.Post.Details(editedPost.Slug));
            }

            return View(CreateAdminPostEdit(editedPost));
        }

        private void CreatePostRevision(Post editedPost)
        {
            //string tagsIds = string.Join(",", editedPost.Tags.Select(x => x.Id).OrderBy(x=>x).ToList());
            //string categoriesIds = string.Join(",", editedPost.Categories.Select(x => x.Id).OrderBy(x=>x).ToList());

            //PostRevision lastRevision = db.PostRevisions.Include(x=>x.Author).OrderByDescending(x=> x.RevisionNumber).FirstOrDefault(x => x.Post.Id == editedPost.Id);

            //if(lastRevision == null || editedPost.Title != lastRevision.Title || editedPost.Description != lastRevision.Description || editedPost.Slug != lastRevision.Slug || editedPost.Content != lastRevision.Content || editedPost.ImageUrl != lastRevision.ImageUrl || editedPost.PublicationDate != lastRevision.PublicationDate || editedPost.IsPublished != lastRevision.IsPublished || editedPost.IsCommentEnabled != lastRevision.IsCommentEnabled || editedPost.Author.Id != lastRevision.Author.Id || tagsIds != lastRevision.TagsIds || categoriesIds != lastRevision.CategoriesIds)
            //{
            //    PostRevision revision = db.PostRevisions.Add(new PostRevision());
            //    revision.RevisionNumber = (lastRevision != null ? lastRevision.RevisionNumber : 0) + 1;
            //    revision.Post = editedPost;
            //    revision.Title = editedPost.Title;
            //    revision.Description = editedPost.Description;
            //    revision.Slug = editedPost.Slug;
            //    revision.Content = editedPost.Content;
            //    revision.ImageUrl = editedPost.ImageUrl;
            //    revision.CreationDate = DateTime.UtcNow;
            //    revision.PublicationDate = editedPost.PublicationDate;
            //    revision.IsPublished = editedPost.IsPublished;
            //    revision.IsCommentEnabled = editedPost.IsCommentEnabled;
            //    revision.Author = editedPost.Author;
            //    revision.TagsIds = tagsIds;
            //    revision.CategoriesIds = categoriesIds;
            //}
            //db.SaveChanges();
        }

        private AdminPostEdit CreateAdminPostEdit(Post post)
        {
            IQueryable<User> authors = db.Users.Where(x => x.Enabled);
            if (!User.IsInRole(Role.EditOtherUsersPosts))
            {
                authors = authors.Where(x => x.Username == User.Identity.Name);
            }

            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Authors = authors.ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Username });

            viewModel.Post = new AdminPostSave(post);
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTagsJson = new JavaScriptSerializer().Serialize(db.Tags.Select(x => x.Name));
            return viewModel;
        }

        private void CheckPostsSaveRoles(AdminPostSave postSave, Post editedPost, User newAuthor)
        {
            if (!postSave.Id.HasValue && !User.IsInRole(Role.CreateNewPosts))
            {
                Response.SendToUnauthorized();
            }

            if (postSave.Id.HasValue && !User.IsInRole(Role.EditOtherUsersPosts) &&
                !(editedPost.Author.Username == User.Identity.Name && User.IsInRole(Role.EditOwnPosts)))
            {
                Response.SendToUnauthorized();
            }

            if (!User.IsInRole(Role.EditOtherUsersPosts) && newAuthor.Username != User.Identity.Name)
            {
                Response.SendToUnauthorized();
            }

            if (postSave.IsPublished && (!postSave.Id.HasValue || !editedPost.IsPublished))
            {
                string authorUserName = newAuthor.Username;

                if (!(authorUserName != User.Identity.Name && User.IsInRole(Role.PublishOtherUsersPosts)) &&
                    !(authorUserName == User.Identity.Name && User.IsInRole(Role.PublishOwnPosts)))
                {
                    Response.SendToUnauthorized();
                }
            }
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            Post deletedPost = db.Posts.Include(x=>x.Author).Single(x => x.Id == id);

            if (!(deletedPost.Author.Username == User.Identity.Name && User.IsInRole(Role.DeleteOwnPosts)) && !(deletedPost.Author.Username != User.Identity.Name && User.IsInRole(Role.DeleteOtherUsersPosts)))
            {
                Response.SendToUnauthorized();
            }

            deletedPost.IsDeleted = true;
            db.SaveChangesBypassingValidation();

            return Json(true);
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