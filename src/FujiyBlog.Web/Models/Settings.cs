using System.Web.Mvc;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Web.Models
{
    public static class Settings
    {
        public static SettingRepository SettingRepository
        {
            get { return DependencyResolver.Current.GetService<SettingRepository>(); }
        }
    }
}