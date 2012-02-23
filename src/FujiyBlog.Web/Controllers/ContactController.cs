using System;
using System.Web.Mvc;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.ViewModels;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Controllers
{
    public partial class ContactController : AbstractController
    {
        public virtual ActionResult Index()
        {
            return View(new ContactForm());
        }
        
        [HttpPost, ActionName("Index")]
        public virtual ActionResult IndexPost(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                string body = contactForm.Body;

                body += Server.HtmlEncode(body).Replace(Environment.NewLine, "<br />") + @"
<br />
<br />
<hr />
<br />
<h3>Author information</h3>";

                body += "<strong>Name:</strong> " + Server.HtmlEncode(contactForm.Name) + "<br />";
                body += "<strong>E-mail:</strong> " + Server.HtmlEncode(contactForm.Email) + "<br />";
                body += "<strong>IP Address:</strong> " + Request.UserHostAddress + "<br />";
                body += "<strong>Browser:</strong> " + Request.UserAgent;

                EmailService.Send(Settings.SettingRepository.EmailTo, contactForm.Subject, body, true, contactForm.Email, contactForm.Name);

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
