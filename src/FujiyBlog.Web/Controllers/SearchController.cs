using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Controllers
{
    public partial class SearchController : AbstractController
    {
        private readonly FujiyBlogDatabase db;

        public SearchController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ActionResult Index(int? page, string terms)
        {
            int skip = (page.GetValueOrDefault(1) - 1) * Settings.SettingRepository.PostsPerPage;
            string[] termsSplit = terms.Split(' ');

            var encodedTerm = Server.HtmlEncode(terms);
            ViewBag.Title = "Search for" + " '" + encodedTerm + "'";

            IQueryable<Post> query = from post in db.Posts.WhereHavePermissions().Include(x => x.Tags).Include(x => x.Categories)
                                     orderby post.PublicationDate descending 
                                     where termsSplit.Any(x => post.Content.Contains(x)) || termsSplit.Any(x => post.Title.Contains(x)) || termsSplit.Any(x => post.Description.Contains(x))
                                     select post;

            SearchResult viewModel = new SearchResult
                                         {
                                             CurrentPage = page.GetValueOrDefault(1),
                                             Posts = query.Skip(skip).Take(Settings.SettingRepository.PostsPerPage).ToList(),
                                             TotalPages = (int) Math.Ceiling(query.Count()/(double) Settings.SettingRepository.PostsPerPage),
                                         };

            return View(viewModel);
        }

    }
}
