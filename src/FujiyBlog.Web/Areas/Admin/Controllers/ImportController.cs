using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.BlogML;
using FujiyBlog.EntityFramework;

namespace FujiyBlog.Web.Areas.Admin.Controllers
{
    public partial class ImportController : AdminController
    {
        private readonly FujiyBlogDatabase db;

        public ImportController(FujiyBlogDatabase db)
        {
            this.db = db;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult ImportBlogML()
        {
            var file = Request.Files[0];

            BlogMLImporter importer = new BlogMLImporter(new BlogMLRepository(db));

            importer.Import(file.InputStream);
            db.SaveChanges();
            return Content("OK");
        }
    }
}
