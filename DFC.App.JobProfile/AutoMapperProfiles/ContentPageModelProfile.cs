using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using dfc_content_pkg_netcore.models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<PagesApiDataModel, ContentPageModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore());

            //CreateMap<ApiContentItemModel, ContentItemModel>()
            //    .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published));

            //CreateMap<ApiContentItemModel, SharedContentItemModel>()
            //    .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published));

            CreateMap<LinkDetails, BaseContentItemModel>()
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore());
        }
    }
}
