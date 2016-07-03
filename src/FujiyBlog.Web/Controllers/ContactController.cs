using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Extensions;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Controllers
{
    public partial class ContactController : Controller
    {
        private readonly SettingRepository settings;

        public ContactController(SettingRepository settings)
        {
            this.settings = settings;
        }

        public virtual ActionResult Index()
        {
            return View(new ContactForm());
        }

        [HttpPost, ActionName("Index")]
        public async virtual Task<ActionResult> IndexPost(ContactForm contactForm)
        {
            if (ModelState.IsValid && (settings.ReCaptchaEnabled == false || HttpContext.UserHasClaimPermission(PermissionClaims.ModerateComments) || await CommentController.ValidateRecaptcha(Request, settings, Request.Form["g-recaptcha-response"])))
            {
                string body = HtmlEncoder.Default.Encode(contactForm.Body).Replace(Environment.NewLine, "<br />") + @"
<br />
<br />
<hr />
<br />
<h3>Author information</h3>";

                body += "<strong>Name:</strong> " + HtmlEncoder.Default.Encode(contactForm.Name) + "<br />";
                body += "<strong>E-mail:</strong> " + HtmlEncoder.Default.Encode(contactForm.Email) + "<br />";
                body += "<strong>IP Address:</strong> " + HttpContext.Connection.RemoteIpAddress.ToString() + "<br />";
                body += "<strong>Browser:</strong> " + Request.Headers["User-Agent"];

                await new EmailService(settings).Send(settings.EmailTo, contactForm.Subject, body, true, contactForm.Email, contactForm.Name);

                return RedirectToAction(nameof(Success));
            }
            return View(contactForm);
        }

        public virtual ActionResult Success()
        {
            return View();
        }
    }
}
