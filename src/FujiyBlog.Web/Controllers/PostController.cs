using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.Services;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Controllers
{
    public partial class PostController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly PostService postService;

        public PostController(IUnitOfWork unitOfWork, IPostRepository postRepository, IUserRepository userRepository, PostService postService)
        {
            this.unitOfWork = unitOfWork;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.postService = postService;
        }

        public virtual ActionResult Index(int? skip)
        {
            PostIndex model = new PostIndex
                                  {
                                      PostsPerPage = Settings.SettingRepository.PostsPerPage,
                                      RecentPosts = postRepository.GetRecentPosts(skip.GetValueOrDefault(), Settings.SettingRepository.PostsPerPage, isPublic: !User.Identity.IsAuthenticated),
                                      TotalPosts = postRepository.GetTotal(isPublic: !User.Identity.IsAuthenticated),
                                  };

            ViewBag.Title = Settings.SettingRepository.BlogName + " - " + Settings.SettingRepository.BlogDescription;
            ViewBag.Description = Settings.SettingRepository.BlogDescription;

            return View(model);
        }

        public virtual ActionResult Tag(string tag, int? skip)
        {
            PostIndex model = new PostIndex
            {
                PostsPerPage = Settings.SettingRepository.PostsPerPage,
                RecentPosts = postRepository.GetRecentPosts(skip.GetValueOrDefault(), Settings.SettingRepository.PostsPerPage, tag, isPublic: !User.Identity.IsAuthenticated),
                TotalPosts = postRepository.GetTotal(tag, isPublic: !User.Identity.IsAuthenticated),
            };

            return View(MVC.Post.Views.Index, model);
        }

        public virtual ActionResult Category(string category, int? skip)
        {
            PostIndex model = new PostIndex
            {
                PostsPerPage = Settings.SettingRepository.PostsPerPage,
                RecentPosts = postRepository.GetRecentPosts(skip.GetValueOrDefault(), Settings.SettingRepository.PostsPerPage, category: category, isPublic: !User.Identity.IsAuthenticated),
                TotalPosts = postRepository.GetTotal(category: category, isPublic: !User.Identity.IsAuthenticated),
            };

            return View(MVC.Post.Views.Index, model);
        }

        public virtual ActionResult Author(string author, int? skip)
        {
            PostIndex model = new PostIndex
            {
                PostsPerPage = Settings.SettingRepository.PostsPerPage,
                RecentPosts = postRepository.GetRecentPosts(skip.GetValueOrDefault(), Settings.SettingRepository.PostsPerPage, authorUserName: author, isPublic: !User.Identity.IsAuthenticated),
                TotalPosts = postRepository.GetTotal(authorUserName: author, isPublic: !User.Identity.IsAuthenticated),
            };

            return View(MVC.Post.Views.Index, model);
        }

        public virtual ActionResult Archive()
        {
            PostArchive model = new PostArchive
            {
                AllPosts = postRepository.GetArchive(!User.Identity.IsAuthenticated)
            };

            model.UncategorizedPosts = model.AllPosts.Where(x => !x.Categories.Any());
            model.AllCategories = model.AllPosts.SelectMany(x => x.Categories).Distinct();
            model.TotalPosts = model.AllPosts.Count();
            model.TotalComments = model.AllPosts.Sum(x => x.CommentsTotal);

            return View(model);
        }

        public virtual ActionResult Details(string postSlug)
        {
            return Details(postSlug, null);
        }

        public virtual ActionResult DetailsById(int id)
        {
            return Details(null, id);
        }

        private ActionResult Details(string slug, int? id)
        {
            Post post = id.HasValue ? postRepository.GetPost(id.GetValueOrDefault(), !User.Identity.IsAuthenticated) : postRepository.GetPost(slug, !User.Identity.IsAuthenticated);

            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = post.Title;
            ViewBag.Keywords = string.Join(",", post.Tags.Select(x => x.Name).Concat(post.Categories.Select(x => x.Name)));
            ViewBag.Description = post.Description;

            Post previousPost = postRepository.GetPreviousPost(post, !User.Identity.IsAuthenticated);
            Post nextPost = postRepository.GetNextPost(post, !User.Identity.IsAuthenticated);

            PostDetails postDetails = new PostDetails
            {
                Post = post,
                PreviousPost = previousPost,
                NextPost = nextPost
            };

            return View(postDetails);
        }

        public virtual ActionResult DoComment(int id)
        {
            bool isLogged = User.Identity.IsAuthenticated;

            PostComment postComment = new PostComment
                                          {
                                              IpAddress = Request.UserHostAddress,
                                              Post = postRepository.GetPost(id, !isLogged)
                                          };

            if (isLogged)
            {
                postComment.Author = userRepository.GetByUsername(User.Identity.Name);
                UpdateModel(postComment, new[] { "Comment" });
            }
            else
            {
                UpdateModel(postComment, new[] {"AuthorName", "AuthorEmail", "AuthorWebsite", "Comment"});
            }

            postService.AddComment(postComment);
            unitOfWork.SaveChanges();
            
            return View(MVC.Post.Views.Comments, new[] { postComment });
        }
    }
}
