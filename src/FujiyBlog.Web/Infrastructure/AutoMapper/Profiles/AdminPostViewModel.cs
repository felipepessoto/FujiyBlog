using AutoMapper;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Areas.Admin.ViewModels;
using System.Linq;
using FujiyBlog.Web.Common;

namespace FujiyBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class AdminPostViewModel : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, AdminPostSave>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => string.Join(", ", src.Tags.Select(x => x.Name))))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeUtil.ConvertUtcToMyTimeZone(src.PublicationDate)));

            Mapper.CreateMap<AdminPostSave, Post>()
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeUtil.ConvertMyTimeZoneToUtc(src.PublicationDate)))
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}