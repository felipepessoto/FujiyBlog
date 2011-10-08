using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Extensions;

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
            IQueryable<Page> pages = db.Pages.Include(x => x.Author).Where(x => !x.IsDeleted);

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
            if (!id.HasValue && !User.IsInRole(Role.CreateNewPages))
            {
                Response.SendToUnauthorized();
            }

            Page page = id.HasValue
                            ? db.Pages.Include(x => x.Author).Single(x => x.Id == id)
                            : new Page
                                  {
                                      PublicationDate = DateTime.UtcNow,
                                      IsPublished = true,
                                      ShowInList = true,
                                  };

            if (id.HasValue && !User.IsInRole(Role.EditOtherUsersPages) && !(page.Author.Username == User.Identity.Name && User.IsInRole(Role.EditOwnPages)))
            {
                Response.SendToUnauthorized();
            }

            IQueryable<User> authors = db.Users.Where(x => x.Enabled);
            if (!User.IsInRole(Role.EditOtherUsersPages))
            {
                authors = authors.Where(x => x.Username == User.Identity.Name);
            }

            AdminPageSave viewModel = Mapper.Map<Page, AdminPageSave>(page);
            viewModel.Authors = authors.ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Username, Selected = (x == page.Author || (!id.HasValue && x.Username == User.Identity.Name)) });
            viewModel.Id = id;
            viewModel.Pages = (from p in db.Pages
                               where p.Id != page.Id
                               select new { p.Id, p.Title }).ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title, Selected = x.Id == page.ParentId });

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public virtual ActionResult EditPost(AdminPageSave pageSave)
        {
            Page editedPage = pageSave.Id.HasValue ? db.Pages.Include(x => x.Author).Single(x => x.Id == pageSave.Id)
                                  : db.Pages.Add(new Page
                                        {
                                            CreationDate = DateTime.UtcNow,
                                        });

            User newAuthor = db.Users.Single(x => x.Id == pageSave.AuthorId);

            CheckPagesSaveRoles(pageSave, editedPage, newAuthor);

            editedPage.Author = newAuthor;

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

            IQueryable<User> authors = db.Users.Where(x => x.Enabled);
            if (!User.IsInRole(Role.EditOtherUsersPages))
            {
                authors = authors.Where(x => x.Username == User.Identity.Name);
            }
            viewModel.Authors = authors.ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Username, Selected = (x == editedPage.Author) });

            viewModel.Pages = (from p in db.Pages
                               where p.Id != editedPage.Id
                               select new { p.Id, p.Title }).ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title, Selected = x.Id == editedPage.ParentId });

            return View(viewModel);
        }

        private void CheckPagesSaveRoles(AdminPageSave pageSave, Page editedPage, User newAuthor)
        {
            if (!pageSave.Id.HasValue && !User.IsInRole(Role.CreateNewPages))
            {
                Response.SendToUnauthorized();
            }

            if (pageSave.Id.HasValue && !User.IsInRole(Role.EditOtherUsersPages) &&
                !(editedPage.Author.Username == User.Identity.Name && User.IsInRole(Role.EditOwnPages)))
            {
                Response.SendToUnauthorized();
            }

            if (!User.IsInRole(Role.EditOtherUsersPages) && newAuthor.Username != User.Identity.Name)
            {
                Response.SendToUnauthorized();
            }

            if (pageSave.IsPublished && (!pageSave.Id.HasValue || !editedPage.IsPublished))
            {
                string authorUserName = newAuthor.Username;

                if (!(authorUserName != User.Identity.Name && User.IsInRole(Role.PublishOtherUsersPages)) &&
                    !(authorUserName == User.Identity.Name && User.IsInRole(Role.PublishOwnPages)))
                {
                    Response.SendToUnauthorized();
                }
            }
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            Page deletedPage = db.Pages.Include(x => x.Author).Single(x => x.Id == id);

            if (!(deletedPage.Author.Username == User.Identity.Name && User.IsInRole(Role.DeleteOwnPages)) && !(deletedPage.Author.Username != User.Identity.Name && User.IsInRole(Role.DeleteOtherUsersPages)))
            {
                Response.SendToUnauthorized();
            }

            deletedPage.IsDeleted = true;

            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
            finally
            {
                db.Configuration.ValidateOnSaveEnabled = true;
            }

            return Json(true);
        }

        [HttpPost]
        public virtual ActionResult GenerateSlug(string text)
        {
            return Content(text.GenerateSlug());
        }
    }
}