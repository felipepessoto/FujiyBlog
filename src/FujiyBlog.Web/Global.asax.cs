using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.Infrastructure;
using NLog;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace FujiyBlog.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MiniProfilerEF6.Initialize();
            
            //AutoDbMigration();

            LogManager.GetCurrentClassLogger().Info("Started FujiyBlog");

            foreach (IViewEngine viewEngine in ViewEngines.Engines.Where(x=> !(x is RazorViewEngine)).ToList())
            {
                ViewEngines.Engines.Remove(viewEngine);
            }

            DependencyResolver.SetResolver(new UnityDependencyResolver());

            MiniProfiler.Settings.Results_Authorize = httpRequest => httpRequest.IsAuthenticated;
            MiniProfiler.Settings.Results_List_Authorize = httpRequest => httpRequest.IsAuthenticated;
        }

        private static void AutoDbMigration()
        {
            System.Data.Entity.Migrations.DbMigrationsConfiguration configuration = new System.Data.Entity.Migrations.DbMigrationsConfiguration();
            configuration.ContextType = typeof (FujiyBlogDatabase);
            configuration.MigrationsAssembly = configuration.ContextType.Assembly;
            configuration.MigrationsNamespace = "FujiyBlog.Core.Migrations";

            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);

            if (false)
            {
                var scriptor = new System.Data.Entity.Migrations.Infrastructure.MigratorScriptingDecorator(migrator);
                string script = scriptor.ScriptUpdate(sourceMigration: null, targetMigration: null);
            }
            else
            {
                migrator.Update();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            MiniProfiler.Start();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            MiniProfiler.Stop();
            using (DependencyResolver.Current as IDisposable)
            {
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            LogManager.GetCurrentClassLogger().Error(Server.GetLastError());
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            string result = string.Empty;

            foreach (string s in arg.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.ToLower() == "user")
                {
                    result += "user:" + context.User.Identity.Name;
                }
                if (s.ToLower() == "lastcache")
                {
                    result += "lastcache:" + DependencyResolver.Current.GetService<FujiyBlogDatabase>().LastDatabaseChange;
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return base.GetVaryByCustomString(context, arg);
        }
    }
}