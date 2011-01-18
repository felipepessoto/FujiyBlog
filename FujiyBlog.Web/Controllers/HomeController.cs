using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IUserRepository userRepository)
        {
            var d= userRepository.Find(0);
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
