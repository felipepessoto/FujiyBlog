using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Areas.Admin.ViewModels;

namespace FujiyBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class AdminUserViewModel : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<AdminUserSave, User>().ForMember(dest => dest.Username, opt => opt.Ignore());
            Mapper.CreateMap<User, AdminUserSave>();
            Mapper.CreateMap<AdminUserCreate, User>();
        }
    }
}