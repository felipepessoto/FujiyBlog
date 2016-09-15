using System;
using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using FujiyBlog.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FujiyBlog.Core;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class PageController : AdminController
    {
        private readonly FujiyBlogDatabase db;
        private const int PageSize = 10;
        private readonly DateTimeUtil dateTimeUtil;

        public PageController(FujiyBlogDatabase db, DateTimeUtil dateTimeUtil)
        {
            this.db = db;
            this.dateTimeUtil = dateTimeUtil;
        }

        public ViewResult Index(int? page, bool? published)
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
                Published = published
            };

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateNewPages))
            {
                return Forbid();
            }

            Page page = id.HasValue ? db.Pages.Include(x => x.Author).Single(x => x.Id == id)
                            : new Page
                                  {
                                      PublicationDate = DateTime.UtcNow,
                                      IsPublished = true,
                                      ShowInList = true,
                                      Author = db.Users.Single(x => x.UserName == User.Identity.Name),
                                  };

            if (id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPages) && !(page.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnPages)))
            {
                return Forbid();
            }

            return View(CreateAdminPageSave(page));
        }

        private AdminPageSave CreateAdminPageSave(Page page)
        {
            IQueryable<ApplicationUser> authors = db.Users.Where(x => x.Enabled);
            if (!HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPages))
            {
                authors = authors.Where(x => x.UserName == User.Identity.Name);
            }

            AdminPageSave viewModel = new AdminPageSave(page, dateTimeUtil);
            viewModel.Authors = authors.ToList().Select(x => new SelectListItem {Value = x.Id.ToString(), Text = x.UserName});
            viewModel.Pages = (from p in db.Pages
                               where p.Id != page.Id
                               select new { p.Id, p.Title }).ToList().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title, Selected = x.Id == page.ParentId });

            return viewModel;
        }

        [HttpPost]
        public ActionResult Edit(AdminPageSave pageSave)
        {
            Page editedPage = pageSave.Id.HasValue ? db.Pages.Include(x => x.Author).Single(x => x.Id == pageSave.Id)
                                  : db.Pages.Add(new Page
                                        {
                                            CreationDate = DateTime.UtcNow,
                                        }).Entity;

            var newAuthor = db.Users.Single(x => x.Id == pageSave.AuthorId);

            if(CheckPagesSaveRoles(pageSave, editedPage, newAuthor) == false)
            {
                return Forbid();
            }

            editedPage.Author = newAuthor;

            editedPage.LastModificationDate = DateTime.UtcNow;

            pageSave.FillPage(editedPage, dateTimeUtil);

            if (db.Pages.Any(x => x.Slug == editedPage.Slug && x.Id != editedPage.Id))
            {
                ModelState.AddModelError("Slug", "This slug already exists");
            }

            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(CreateAdminPageSave(editedPage));
        }

        private bool CheckPagesSaveRoles(AdminPageSave pageSave, Page editedPage, ApplicationUser newAuthor)
        {
            if (!pageSave.Id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.CreateNewPages))
            {
                return false;
            }

            if (pageSave.Id.HasValue && !HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPages) &&
                !(editedPage.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.EditOwnPages)))
            {
                return false;
            }

            if (!HttpContext.UserHasClaimPermission(PermissionClaims.EditOtherUsersPages) && newAuthor.UserName != User.Identity.Name)
            {
                return false;
            }

            if (pageSave.IsPublished && (!pageSave.Id.HasValue || !editedPage.IsPublished))
            {
                string authorUserName = newAuthor.UserName;

                if (!(authorUserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.PublishOtherUsersPages)) &&
                    !(authorUserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.PublishOwnPages)))
                {
                    return false;
                }
            }

            return true;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Page deletedPage = db.Pages.Include(x => x.Author).Single(x => x.Id == id);

            if (!(deletedPage.Author.UserName == User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.DeleteOwnPages)) && !(deletedPage.Author.UserName != User.Identity.Name && HttpContext.UserHasClaimPermission(PermissionClaims.DeleteOtherUsersPages)))
            {
                return Forbid();
            }

            deletedPage.IsDeleted = true;
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult GenerateSlug(string text)
        {
            return Content(text.GenerateSlug());
        }
    }
}