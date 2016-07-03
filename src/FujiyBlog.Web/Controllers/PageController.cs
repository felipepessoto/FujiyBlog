using System;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public class PageController : Controller
    {
        private readonly FujiyBlogDatabase db;

        public PageController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            Page initialPage = db.Pages.WhereHaveRoles(HttpContext).Single(x => x.IsFrontPage);
            return ShowPage(initialPage);
        }

        public ActionResult Details(string pageSlug)
        {
            return Details(pageSlug, null);
        }

        public ActionResult DetailsById(int id)
        {
            return Details(null, id);
        }

        private ActionResult Details(string slug, int? id)
        {
            IQueryable<Page> pageQuery = db.Pages.WhereHaveRoles(HttpContext);

            Page page;

            if (id.HasValue)
            {
                page = pageQuery.SingleOrDefault(x => x.Id == id.Value);
            }
            else
            {
                page = pageQuery.SingleOrDefault(x => x.Slug == slug);
            }

            if (page == null)
            {
                return NotFound();
            }

            return ShowPage(page);
        }

        private ActionResult ShowPage(Page page)
        {
            ViewBag.Title = page.Title;
            ViewBag.Keywords = page.Keywords;
            ViewBag.Description = page.Description;

            return View("Details", page);
        }
    }
}
