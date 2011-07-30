using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PageController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private const int PageSize = 10;

        public PageController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ViewResult Index(int? page, bool? published)
        {
            IQueryable<Page> pages = db.Pages.Where(x => !x.IsDeleted);

            if (published.HasValue)
            {
                pages = published.Value ? pages.Where(x => x.IsPublished) : pages.Where(x => !x.IsPublished);
            }

            IQueryable<Page> pagePages = pages.OrderByDescending(x => x.PublicationDate).Paging(page.GetValueOrDefault(1), PageSize);

            AdminPageIndex model = new AdminPageIndex
            {
                CurrentPage = page.GetValueOrDefault(1),
                Pages = pagePages.ToList(),
                TotalPages = (int)Math.Ceiling((double)pages.Count() / PageSize),
            };

            return View(model);
        }

        public virtual ActionResult Edit(int? id)
        {
            Page page = id.HasValue
                            ? db.Pages.Single(x => x.Id == id)
                            : new Page
                                  {
                                      PublicationDate = DateTime.UtcNow,
                                      IsPublished = true,
                                      ShowInList = true,
                                  };


            AdminPageSave viewModel = Mapper.Map<Page, AdminPageSave>(page);
            viewModel.Id = id;
            viewModel.Pages = (from p in db.Pages
                               where p.Id != page.Id
                               select new { p.Id, p.Title }).ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title, Selected = x.Id == page.ParentId });

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public virtual ActionResult EditPost(AdminPageSave pageSave)
        {
            Page editedPage = pageSave.Id.HasValue ? db.Pages.Single(x => x.Id == pageSave.Id)
                                  : db.Pages.Add(new Page
                                        {
                                            CreationDate = DateTime.UtcNow,
                                        });

            editedPage.LastModificationDate = DateTime.UtcNow;

            Mapper.Map(pageSave, editedPage);

            if (db.Pages.Any(x => x.Slug == editedPage.Slug && x.Id != editedPage.Id))
            {
                ModelState.AddModelError("Slug", "This slug already exists");
            }

            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            AdminPageSave viewModel = Mapper.Map<Page, AdminPageSave>(editedPage);

            viewModel.Pages = (from p in db.Pages
                               where p.Id != editedPage.Id
                               select new { p.Id, p.Title }).ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title, Selected = x.Id == editedPage.ParentId });

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            db.Pages.Find(id).IsDeleted = true;
            db.SaveChanges();
            return Json(true);
        }
    }
}