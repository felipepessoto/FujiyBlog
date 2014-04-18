using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FujiyBlog.Web.Startup))]
namespace FujiyBlog.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
