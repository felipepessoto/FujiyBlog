using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FujiyBlog.Web.Controllers
{
    //TODO [NonAuthenticatedOnlyCache(CacheProfile = "ByUserAndLastCache")]
    public class PostController : Controller
    {
        private readonly PostRepository postRepository;
        private readonly FujiyBlogDatabase db;
        private readonly SettingRepository settings;

        public PostController(PostRepository postRepository, SettingRepository settings, FujiyBlogDatabase db)
        {
            this.postRepository = postRepository;
            this.settings = settings;
            this.db = db;
        }

        public ActionResult Index(int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = GetRecentPostsCache(skip, settings.PostsPerPage, null, null, null, null, null),
                TotalPages = (int)Math.Ceiling(GetTotalCache(null, null, null, null, null) / (double)settings.PostsPerPage),
            };

            if (page > model.TotalPages)
            {
                return NotFound();
            }

            ViewBag.Title = settings.BlogName + " - " + settings.BlogDescription;
            ViewBag.Description = settings.BlogDescription;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
                ViewBag.Description += " - Page " + page.Value;
            }


            return View(model);
        }

        public ActionResult Tag(string tag, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = GetRecentPostsCache(skip, settings.PostsPerPage, tag),
                TotalPages = (int)Math.Ceiling(GetTotalCache(tag) / (double)settings.PostsPerPage),
            };

            if (page > model.TotalPages || !model.RecentPosts.Any())
            {
                return NotFound();
            }

            ViewBag.Title = "All posts tagged '" + tag + "'";

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

            return View("Index", model);
        }

        public ActionResult Category(string category, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = GetRecentPostsCache(skip, settings.PostsPerPage, category: category),
                TotalPages = (int)Math.Ceiling(GetTotalCache(category: category) / (double)settings.PostsPerPage),
            };

            if (page > model.TotalPages || !model.RecentPosts.Any())
            {
                return NotFound();
            }

            ViewBag.Title = category;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

            return View("Index", model);
        }

        public ActionResult Author(string author, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = GetRecentPostsCache(skip, settings.PostsPerPage, authorUserName: author),
                TotalPages = (int)Math.Ceiling(GetTotalCache(authorUserName: author) / (double)settings.PostsPerPage),
            };

            var authorUser = db.Users.SingleOrDefault(x => x.UserName == author && x.Enabled == true);

            if (page > model.TotalPages || authorUser == null)
            {
                return NotFound();
            }

            ViewBag.Title = "All posts by '" + (authorUser.DisplayName ?? authorUser.UserName) + "'";
            ViewBag.Description = authorUser.About;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
                ViewBag.Description += " - Page " + page.Value;
            }

            return View("Index", model);
        }

        private IEnumerable<PostSummary> GetRecentPostsCache(int skip, int take, string tag = null, string category = null, string authorUserName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            string recentPostsCacheKey = $"FujiyBlog.Core.EntityFramework.PostRepository.GetRecentPosts(int, int, string, string, string, DateTime?, DateTime?). Params:{skip}, {take}, {tag}, {category}, {authorUserName}, {startDate?.ToString("O")}, {endDate?.ToString("O")})";

            var result = CacheHelper.FromCacheOrExecute(db, () => postRepository.GetRecentPosts(skip, settings.PostsPerPage, tag, category, authorUserName, startDate, endDate), recentPostsCacheKey, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, !User.Identity.IsAuthenticated);

            return result;
        }

        private int GetTotalCache(string tag = null, string category = null, string authorUserName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            string getTotalCacheKey = $"FujiyBlog.Core.EntityFramework.PostRepository.GetTotal(string, string, string, DateTime?, DateTime?). Params:{tag}, {category}, {authorUserName}, {startDate?.ToString("O")}, {endDate?.ToString("O")})";

            var result = CacheHelper.FromCacheOrExecute(db, () => postRepository.GetTotal(tag, category, authorUserName, startDate, endDate), getTotalCacheKey, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }, !User.Identity.IsAuthenticated);

            return result;
        }

        public ActionResult Archive()
        {
            PostArchive model = new PostArchive
            {
                AllPosts = CacheHelper.FromCacheOrExecute(db, () => postRepository.GetArchive(), cacheItemPolicy: new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, condition: !User.Identity.IsAuthenticated)
            };

            model.UncategorizedPosts = model.AllPosts.Where(x => !x.Post.PostCategories.Any()).ToList();
            model.AllCategories = model.AllPosts.SelectMany(x => x.Post.PostCategories.Select(y => y.Category)).Distinct().ToList();
            model.TotalPosts = model.AllPosts.Count();
            model.TotalComments = model.AllPosts.Sum(x => x.CommentsTotal);

            return View(model);
        }

        public ActionResult ArchiveDate(int year, int month, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = GetRecentPostsCache(skip, settings.PostsPerPage, startDate: startDate, endDate: endDate),
                TotalPages = (int)Math.Ceiling(GetTotalCache(startDate: startDate, endDate: endDate) / (double)settings.PostsPerPage),
            };

            if (page > model.TotalPages)
            {
                return NotFound();
            }

            ViewBag.Title = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " - " + year;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

            return View("Index", model);
        }

        public ActionResult Details(string postSlug)
        {
            Post post = CacheHelper.FromCacheOrExecute(db, () => db.GetCompletePost(postSlug, HttpContext), "FujiyBlog.Core.Extensions.QueryableExtensions.GetCompletePost(" + postSlug + ") as " + User.Identity.Name);

            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Title = post.Title;
            ViewBag.Keywords = string.Join(",", post.PostTags.Select(x => x.Tag.Name).Concat(post.PostCategories.Select(x => x.Category.Name)));
            ViewBag.Description = post.Description;

            Post previousPost = CacheHelper.FromCacheOrExecute(db, () => db.Posts.GetPreviousPost(post, HttpContext), "FujiyBlog.Core.Extensions.QueryableExtensions.GetPreviousPost(" + post.Id + ") as " + User.Identity.Name);
            Post nextPost = CacheHelper.FromCacheOrExecute(db, () => db.Posts.GetNextPost(post, HttpContext), "FujiyBlog.Core.Extensions.QueryableExtensions.GetNextPost(" + post.Id + ") as " + User.Identity.Name);

            PostSummary postDetails = new PostSummary
            {
                Post = post,
                ShowFullPost = true,
                CommentsTotal = post.Comments.Count,
                PreviousPost = previousPost,
                NextPost = nextPost,
            };

            return View("Details", postDetails);
        }

        public ActionResult DetailsById(int id)
        {
            string postSlug = db.Posts.Where(x => x.Id == id).Select(x => x.Slug).SingleOrDefault();

            return RedirectToActionPermanent(nameof(Details), new { postSlug });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
