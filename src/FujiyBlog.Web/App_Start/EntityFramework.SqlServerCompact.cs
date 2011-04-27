using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(FujiyBlog.Web.App_Start.EntityFramework_SqlServerCompact), "Start")]

namespace FujiyBlog.Web.App_Start {
    public static class EntityFramework_SqlServerCompact {
        public static void Start() {
            if (ConfigurationManager.ConnectionStrings["FujiyBlogDatabase"] != null && ConfigurationManager.ConnectionStrings["FujiyBlogDatabase"].ProviderName == "System.Data.SqlServerCe.4.0")
            {
                Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            }
        }
    }
}
