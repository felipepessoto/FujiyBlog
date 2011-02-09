using System.Web.Mvc;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Web.Models
{
    public static class Settings
    {
        public static ISettingRepository SettingRepository
        {
            get { return DependencyResolver.Current.GetService<ISettingRepository>(); }
        }
    }
}