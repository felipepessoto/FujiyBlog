using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;
using FujiyBlog.Web.Infrastructure;
using System.Web.Security;

namespace FujiyBlog.Web.Controllers
{
    public partial class PostController : AbstractController
    {
        private readonly PostRepository postRepository;

        public PostController(PostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public virtual ActionResult Index(int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;
            
            PostIndex model = new PostIndex
                                  {
                                      CurrentPage = page.GetValueOrDefault(1),
                                      RecentPosts = CacheHelper.FromCacheOrExecute(() => postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, null, null, null, null, null), cacheItemPolicy: new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, condition: !User.Identity.IsAuthenticated),
                                      TotalPages =  (int)Math.Ceiling(CacheHelper.FromCacheOrExecute(() =>postRepository.GetTotal(null, null, null, null, null), cacheItemPolicy: new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, condition: !User.Identity.IsAuthenticated) / (double)Settings.SettingRepository.PostsPerPage),
                                  };

            ViewBag.Title = Settings.SettingRepository.BlogName + " - " + Settings.SettingRepository.BlogDescription;
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View(model);
        }

        public virtual ActionResult Tag(string tag, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, tag),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(tag) / (double)Settings.SettingRepository.PostsPerPage),
            };

            ViewBag.Title = "All posts tagged '" + tag + "'";
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View("Index", model);
        }

        public virtual ActionResult Category(string category, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, category: category),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(category: category) / (double)Settings.SettingRepository.PostsPerPage),
            };

            ViewBag.Title = category;
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View("Index", model);
        }

        public virtual ActionResult Author(string author, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, authorUserName: author),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(authorUserName: author) / (double)Settings.SettingRepository.PostsPerPage),
            };

            ViewBag.Title = "All posts by '" + author + "'";
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View("Index", model);
        }

        public virtual ActionResult Archive()
        {
            PostArchive model = new PostArchive
            {
                AllPosts = postRepository.GetArchive()
            };

            model.UncategorizedPosts = model.AllPosts.Where(x => !x.Post.Categories.Any()).ToList();
            model.AllCategories = model.AllPosts.SelectMany(x => x.Post.Categories).Distinct().ToList();
            model.TotalPosts = model.AllPosts.Count();
            model.TotalComments = model.AllPosts.Sum(x => x.CommentsTotal);

            return View(model);
        }

        public virtual ActionResult ArchiveDate(int year, int month, int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            PostIndex model = new PostIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                RecentPosts = postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, startDate: startDate, endDate: endDate),
                TotalPages = (int)Math.Ceiling(postRepository.GetTotal(startDate: startDate, endDate: endDate) / (double)Settings.SettingRepository.PostsPerPage),
            };

            ViewBag.Title = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View("Index", model);
        }

        public virtual ActionResult Details(string postSlug)
        {
            if(postSlug.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Details", new {postSlug = postSlug.Substring(0, postSlug.Length - 5)});
            }

            return Details(postSlug, null);
        }

        public virtual ActionResult DetailsById(int id)
        {
            return Details(null, id);
        }

        private ActionResult Details(string slug, int? id)
        {
            Post post = id.HasValue ? postRepository.GetPost(id.GetValueOrDefault()) : postRepository.GetPost(slug);

            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = post.Title;
            ViewBag.Keywords = string.Join(",", post.Tags.Select(x => x.Name).Concat(post.Categories.Select(x => x.Name)));
            ViewBag.Description = post.Description;

            Post previousPost = postRepository.GetPreviousPost(post, !Request.IsAuthenticated);
            Post nextPost = postRepository.GetNextPost(post, !Request.IsAuthenticated);

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
    }
}
