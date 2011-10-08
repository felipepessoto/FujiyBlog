using System;
using System.Linq;
using System.Web.Mvc;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;

namespace FujiyBlog.Web.Controllers
{
    public partial class PageController : AbstractController
    {
        private readonly FujiyBlogDatabase db;

        public PageController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ActionResult Index()
        {
            Page initialPage = db.Pages.WhereHaveRoles().Single(x => x.IsFrontPage);
            return ShowPage(initialPage);
        }

        public virtual ActionResult Details(string pageSlug)
        {
            if (pageSlug.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Details", new { pageSlug = pageSlug.Substring(0, pageSlug.Length - 5) });
            }

            return Details(pageSlug, null);
        }

        public virtual ActionResult DetailsById(int id)
        {
            return Details(null, id);
        }

        private ActionResult Details(string slug, int? id)
        {
            IQueryable<Page> pageQuery = db.Pages.WhereHaveRoles();

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
                return HttpNotFound();
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
