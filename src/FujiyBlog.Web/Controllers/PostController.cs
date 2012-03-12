using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Dto;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;
using FujiyBlog.Core.Extensions;

namespace FujiyBlog.Web.Controllers
{
    [OutputCache(CacheProfile = "ByUserAndLastCache")]
    public partial class PostController : AbstractController
    {
        private readonly PostRepository postRepository;
        private readonly FujiyBlogDatabase db;

        public PostController(PostRepository postRepository, FujiyBlogDatabase db)
        {
            this.postRepository = postRepository;
            this.db = db;
        }

        public virtual ActionResult Index(int? page)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;
            
            PostIndex model = new PostIndex
                                  {
                                      CurrentPage = page.GetValueOrDefault(1),
                                      RecentPosts = CacheHelper.FromCacheOrExecute(() => postRepository.GetRecentPosts(skip, Settings.SettingRepository.PostsPerPage, null, null, null, null, null), cacheItemPolicy: new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, condition: !User.Identity.IsAuthenticated),
                                      TotalPages =  (int)Math.Ceiling(CacheHelper.FromCacheOrExecute(() =>postRepository.GetTotal(null, null, null, null, null), cacheItemPolicy: new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }, condition: !User.Identity.IsAuthenticated) / (double)Settings.SettingRepository.PostsPerPage),
                                  };

            if (page > model.TotalPages)
            {
                return HttpNotFound();
            }

            ViewBag.Title = Settings.SettingRepository.BlogName + " - " + Settings.SettingRepository.BlogDescription;
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
                ViewBag.Description += " - Page " + page.Value;
            }


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

            if (page > model.TotalPages || !model.RecentPosts.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Title = "All posts tagged '" + tag + "'";

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

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

            if (page > model.TotalPages || !model.RecentPosts.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Title = category;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

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

            User authorUser = db.Users.SingleOrDefault(x => x.Username == author && x.Enabled == true);

            if (page > model.TotalPages || authorUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "All posts by '" + (authorUser.DisplayName ?? authorUser.Username) + "'";
            ViewBag.Description = authorUser.About;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
                ViewBag.Description += " - Page " + page.Value;
            }

            return View("Index", model);
        }

        public virtual ActionResult Archive()
        {
            PostArchive model = new PostArchive
            {
                AllPosts = CacheHelper.FromCacheOrExecute(() => postRepository.GetArchive(), cacheItemPolicy: new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) }, condition: !User.Identity.IsAuthenticated)
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

            if (page > model.TotalPages)
            {
                return HttpNotFound();
            }

            ViewBag.Title = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " - " + year;

            if (page.HasValue)
            {
                ViewBag.Title += " - Page " + page.Value;
            }

            ViewBag.Description = ViewBag.Title;

            return View("Index", model);
        }

        public virtual ActionResult Details(string postSlug)
        {
            Post post = CacheHelper.FromCacheOrExecute(() => db.GetCompletePost(postSlug), "FujiyBlog.Core.Extensions.QueryableExtensions.GetCompletePost(" + postSlug + ") as " + User.Identity.Name);

            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = post.Title;
            ViewBag.Keywords = string.Join(",", post.Tags.Select(x => x.Name).Concat(post.Categories.Select(x => x.Name)));
            ViewBag.Description = post.Description;

            Post previousPost = CacheHelper.FromCacheOrExecute(() => db.Posts.GetPreviousPost(post), "FujiyBlog.Core.Extensions.QueryableExtensions.GetPreviousPost(" + post.Id + ") as " + User.Identity.Name);
            Post nextPost = CacheHelper.FromCacheOrExecute(() => db.Posts.GetNextPost(post), "FujiyBlog.Core.Extensions.QueryableExtensions.GetNextPost(" + post.Id + ") as " + User.Identity.Name);

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

        public virtual ActionResult DetailsById(int id)
        {
            string postSlug = db.Posts.Where(x => x.Id == id).Select(x => x.Slug).SingleOrDefault();

            return RedirectToActionPermanent(ActionNames.Details, Name, new { postSlug });
        }
    }
}
