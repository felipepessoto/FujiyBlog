using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Tasks;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FujiyBlog.Web.Controllers
{
    public partial class ContactController : AbstractController
    {
        public virtual ActionResult Index()
        {
            return View(new ContactForm());
        }

        [HttpPost, ActionName("Index")]
        public async virtual Task<ActionResult> IndexPost(ContactForm contactForm)
        {
            if (ModelState.IsValid && (Settings.SettingRepository.ReCaptchaEnabled == false || User.IsInRole(Role.ModerateComments) || await CommentController.ValidateRecaptcha(Request, Request.Form["g-recaptcha-response"])))
            {
                string body = Server.HtmlEncode(contactForm.Body).Replace(Environment.NewLine, "<br />") + @"
<br />
<br />
<hr />
<br />
<h3>Author information</h3>";

                body += "<strong>Name:</strong> " + Server.HtmlEncode(contactForm.Name) + "<br />";
                body += "<strong>E-mail:</strong> " + Server.HtmlEncode(contactForm.Email) + "<br />";
                body += "<strong>IP Address:</strong> " + Request.UserHostAddress + "<br />";
                body += "<strong>Browser:</strong> " + Request.UserAgent;

                new SendEmailTask(Settings.SettingRepository.EmailTo, contactForm.Subject, body, contactForm.Email, contactForm.Name).ExcuteLater();

                return RedirectToAction(MVC.Contact.Success());
            }
            return View(contactForm);
        }

        public virtual ActionResult Success()
        {
            return View();
        }
    }
}
