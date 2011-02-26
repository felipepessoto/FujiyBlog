using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.BlogML;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using System.IO;
using System.Data.Entity.Validation;

namespace FujiyBlog.Web.Controllers
{
    public class TestesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlogMLRepository blogMLRepository;

        public TestesController(IUnitOfWork unitOfWork, IBlogMLRepository blogMLRepository)
        {
            this.unitOfWork = unitOfWork;
            this.blogMLRepository = blogMLRepository;
        }

        //
        // GET: /Testes/

        public ActionResult Index()
        {
            BlogMLImporter blogMLImporter = new BlogMLImporter(blogMLRepository);
            using (Stream s = new FileStream(@"C:\Users\Fujiy\Desktop\BlogML.xml", FileMode.Open))
            {
                blogMLImporter.Import(s);
                try
                {
                    unitOfWork.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    
                }
            }
            return null;
        }

    }
}
