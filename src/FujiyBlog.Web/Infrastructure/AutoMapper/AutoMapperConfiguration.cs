using AutoMapper;
using FujiyBlog.Web.Infrastructure.AutoMapper.Profiles;

namespace FujiyBlog.Web.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new AdminUserViewModel());
            Mapper.AddProfile(new AdminPostViewModel());
        }
    }
}