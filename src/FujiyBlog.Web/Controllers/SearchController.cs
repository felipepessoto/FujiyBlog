using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.ViewModels.SearchViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Encodings.Web;

namespace FujiyBlog.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly FujiyBlogDatabase db;
        private readonly SettingRepository settings;

        public SearchController(FujiyBlogDatabase db, SettingRepository settings)
        {
            this.db = db;
            this.settings = settings;
        }

        public ActionResult Index(int? page, string terms)
        {
            if (string.IsNullOrEmpty(terms))
            {
                return RedirectToAction("Index", "Post");
            }

            int skip = (page.GetValueOrDefault(1) - 1) * settings.PostsPerPage;
            //workaround EF core doesn't translate termsSplit.Any(x => post.Content.Contains(x))
            //string[] termsSplit = terms.Split(' ');

            var encodedTerm = HtmlEncoder.Default.Encode(terms);
            ViewBag.Title = "Search for" + " '" + encodedTerm + "'";

            IQueryable<Post> query = from post in db.Posts.WhereHaveRoles(HttpContext).Include(x => x.PostTags).ThenInclude(x => x.Tag).Include(x => x.PostCategories).ThenInclude(x => x.Category)
                                     orderby post.PublicationDate descending
                                     where post.Content.Contains(terms) || post.Title.Contains(terms) || post.Description.Contains(terms)
                                     select post;

            int count = query.Count();

            SearchResult viewModel = new SearchResult
            {
                CurrentPage = page.GetValueOrDefault(1),
                Posts = query.Skip(skip).Take(settings.PostsPerPage).ToList(),
                TotalPages = (int)Math.Ceiling(count / (double)settings.PostsPerPage),
                Count = count,
                Terms = terms
            };

            return View(viewModel);
        }

    }
}
