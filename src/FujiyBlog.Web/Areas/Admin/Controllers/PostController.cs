using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public class PostController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private readonly PostRepository postRepository;
        private readonly UserRepository userRepository;
        private const int PageSize = 10;
        private readonly DateTimeUtil dateTimeUtil;

        public PostController(FujiyBlogDatabase db, PostRepository postRepository, UserRepository userRepository, DateTimeUtil dateTimeUtil)
        {
            this.db = db;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.dateTimeUtil = dateTimeUtil;
        }

        public ViewResult Index(int? page, bool? published)
        {
            IQueryable<Post> posts = db.Posts.Where(x => !x.IsDeleted);

            if (published.HasValue)
            {
                posts = published.Value ? posts.Where(x => x.IsPublished) : posts.Where(x => !x.IsPublished);
            }

            IQueryable<Post> pagePosts = posts.OrderByDescending(x => x.PublicationDate).Include(x => x.Author).Include(x => x.PostTags).ThenInclude(x => x.Tag).Include(x => x.PostCategories).ThenInclude(x => x.Category)
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
                Published = published
            };

            return View(model);
        }

        public virtual ActionResult Edit(int? id)
        {
            if (!id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateNewPosts))
            {
                return Forbid();
            }

            Post post = id.HasValue ? db.Posts.Include(x => x.PostTags).ThenInclude(x => x.Tag).Include(x => x.PostCategories).ThenInclude(x => x.Category).Include(x => x.Author).Single(x => x.Id == id)
                            : new Post
                            {
                                PublicationDate = DateTime.UtcNow,
                                IsCommentEnabled = true,
                                IsPublished = true,
                                Author = db.Users.Single(x => x.UserName == User.Identity.Name),
                            };

            if (id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPosts) && !(post.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnPosts)))
            {
                return Forbid();
            }

            return View(CreateAdminPostEdit(post));
        }

        [HttpPost]
        public virtual ActionResult Edit([Bind(Prefix = "Post")]AdminPostSave postSave)
        {
            Post editedPost = postSave.Id.HasValue ? db.Posts.Include(x => x.Author).Include(x => x.PostTags).ThenInclude(x => x.Tag).Include(x => x.PostCategories).ThenInclude(x => x.Category).Single(x => x.Id == postSave.Id)
                                  : db.Posts.Add(new Post
                                  {
                                      CreationDate = DateTime.UtcNow,
                                      Slug = postSave.Slug,
                                  }).Entity;

            var newAuthor = userRepository.GetById(postSave.AuthorId); //postSave.AuthorId.HasValue ? userRepository.GetById(postSave.AuthorId.Value) : userRepository.GetByUsername(User.Identity.Name);

            if (CheckPostsSaveRoles(postSave, editedPost, newAuthor) == false)
            {
                return Forbid();
            }

            editedPost.Author = newAuthor;
            editedPost.LastModificationDate = DateTime.UtcNow;
            postSave.FillPost(editedPost, dateTimeUtil);

            if (db.Posts.Any(x => x.Slug == editedPost.Slug && x.Id != editedPost.Id))
            {
                ModelState.AddModelError("Post.Slug", "This slug already exists");
            }

            IEnumerable<string> tags = from tag in (postSave.Tags ?? string.Empty).Split(new[] { ',' })
                                       where !string.IsNullOrWhiteSpace(tag)
                                       select tag.Trim();

            var existingTags = editedPost.PostTags.Select(x => x.Tag.Name).ToList();

            foreach (var removedTag in editedPost.PostTags.Where(x => tags.Contains(x.Tag.Name, StringComparer.OrdinalIgnoreCase) == false).ToList())
            {
                editedPost.PostTags.Remove(removedTag);
            }

            foreach (var item in postRepository.GetOrCreateTags(tags.Where(x => existingTags.Contains(x, StringComparer.OrdinalIgnoreCase) == false)))
            {
                editedPost.PostTags.Add(new PostTag() { Tag = item });
            }

            postSave.SelectedCategories = postSave.SelectedCategories ?? Enumerable.Empty<int>();
            var categoriesToAdd = postSave.SelectedCategories.Except(editedPost.PostCategories.Select(x => x.CategoryId));

            foreach (var removedCategory in editedPost.PostCategories.Where(x => postSave.SelectedCategories.Contains(x.CategoryId) == false).ToList())
            {
                editedPost.PostCategories.Remove(removedCategory);
            }

            foreach (Category category in db.Categories.Where(x => categoriesToAdd.Contains(x.Id)))
            {
                editedPost.PostCategories.Add(new PostCategory() { Category = category });
            }

            if (ModelState.IsValid)
            {
                db.SaveChanges();
                CreatePostRevision(editedPost);
                return RedirectToAction("Details", "Post", new { postSlug = editedPost.Slug });
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
            IQueryable<ApplicationUser> authors = db.Users.Where(x => x.Enabled);
            if (!HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPosts))
            {
                authors = authors.Where(x => x.UserName == User.Identity.Name);
            }

            AdminPostEdit viewModel = new AdminPostEdit();
            viewModel.Authors = authors.ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.UserName });

            viewModel.Post = new AdminPostSave(post, dateTimeUtil);
            viewModel.AllCategories = db.Categories.ToList();
            viewModel.AllTagsJson = JsonConvert.SerializeObject(db.Tags.Select(x => x.Name));
            return viewModel;
        }

        private bool CheckPostsSaveRoles(AdminPostSave postSave, Post editedPost, ApplicationUser newAuthor)
        {
            if (!postSave.Id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateNewPosts))
            {
                return false;
            }

            if (postSave.Id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPosts) &&
                !(editedPost.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnPosts)))
            {
                return false;
            }

            if (!HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPosts) && newAuthor.UserName != User.Identity.Name)
            {
                return false;
            }

            if (postSave.IsPublished && (!postSave.Id.HasValue || !editedPost.IsPublished))
            {
                string authorUserName = newAuthor.UserName;

                if (!(authorUserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.PublishOtherUsersPosts)) &&
                    !(authorUserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.PublishOwnPosts)))
                {
                    return false;
                }
            }
            return true;
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            Post deletedPost = db.Posts.Include(x => x.Author).Single(x => x.Id == id);

            if (!(deletedPost.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.DeleteOwnPosts)) && !(deletedPost.Author.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.DeleteOtherUsersPosts)))
            {
                return Forbid();
            }

            deletedPost.IsDeleted = true;
            db.SaveChanges();

            return Json(true);
        }

        public virtual ActionResult Categories()
        {
            Dictionary<Category, int> categoriesPostCount = (from category in db.Categories
                                                             orderby category.Name
                                                             select new { Category = category, PostCount = category.PostCategories.Where(x => !x.Post.IsDeleted).Count() }).ToDictionary(e => e.Category, e => e.PostCount);

            AdminCategoriesList adminCategoriesList = new AdminCategoriesList { CategoriesPostCount = categoriesPostCount };
            return View(adminCategoriesList);
        }

        [HttpPost]
        public virtual ActionResult UpdateCategory(int id, string name)
        {
            if (db.Categories.Any(x => x.Id != id && x.Name == name))
            {
                return Json(new { errorMessage = "The category already exist" });
            }

            Category category = db.Categories.Single(x => x.Id == id);

            category.Name = name;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult AddCategory([Bind("Name", Prefix = "NewCategory")]Category newCategory)
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
        public ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Single(x => x.Id == id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return Json(true);
        }

        public ActionResult Tags()
        {
            Dictionary<Tag, int> tagsPostCount = (from tag in db.Tags
                                                  orderby tag.Name
                                                  select new { Tag = tag, PostCount = tag.PostTags.Count() }).ToDictionary(e => e.Tag, e => e.PostCount);

            AdminTagsList tagsList = new AdminTagsList { TagsPostCount = tagsPostCount };
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