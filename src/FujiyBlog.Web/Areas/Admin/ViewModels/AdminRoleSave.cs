using System.Collections.Generic;
using System.Security.Claims;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminRoleSave
    {
        public string Id { get; set; }


        public string Name { get; set; }


        public IEnumerable<string> Claims { get; set; }
    }
}
