using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using System.Linq;
using FujiyBlog.Web.Common;

namespace FujiyBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class AdminPageViewModel : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Page, AdminPageSave>()
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeUtil.ConvertUtcToMyTimeZone(src.PublicationDate)));

            Mapper.CreateMap<AdminPageSave, Page>()
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeUtil.ConvertMyTimeZoneToUtc(src.PublicationDate)));
                
        }
    }
}