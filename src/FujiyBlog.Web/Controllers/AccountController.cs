using System;
using System.Web.Mvc;
using System.Web.Security;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Models;
using System.Linq;

namespace FujiyBlog.Web.Controllers
{
    public partial class AccountController : Controller
    {
        private readonly UserService userService;
        private readonly FujiyBlogDatabase db;

        public AccountController(UserService userService, FujiyBlogDatabase db)
        {
            this.userService = userService;
            this.db = db;
        }

        public virtual ActionResult LogOn()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(MVC.Account.ChangePassword());
            }
            return View();
        }

        [HttpPost]
        public virtual ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.SingleOrDefault(x => x.Username == model.UserName);

                if (user != null && !user.Enabled)
                {
                    ModelState.AddModelError("", "The user is disabled.");
                    return View(model);
                }

                if (user != null && user.Password == model.Password)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                    db.SaveChanges();
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(MVC.Post.Index());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction(MVC.Post.Index());
        }

        [Authorize]
        public virtual ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = Settings.SettingRepository.MinRequiredPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (userService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction(MVC.Account.ChangePasswordSuccess());
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = Settings.SettingRepository.MinRequiredPasswordLength;
            return View(model);
        }

        public virtual ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public virtual ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult ForgotPassword(string email)
        {
            User user = db.Users.SingleOrDefault(x => x.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("email", "Email does not exist in our system");
                return View();
            }
            userService.RetrievePassword(user);
            return RedirectToAction(MVC.Account.ForgotPasswordSuccess());
        }

        public virtual ActionResult ForgotPasswordSuccess()
        {
            return View();
        }
    }
}
