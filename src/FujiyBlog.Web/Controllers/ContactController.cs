using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Web.ViewModels;

namespace FujiyBlog.Web.Controllers
{
    public partial class ContactController : AbstractController
    {
        public virtual ActionResult Index()
        {
            return View(new ContactForm());
        }

    }
}
