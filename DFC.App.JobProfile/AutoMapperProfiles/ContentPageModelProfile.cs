using AutoMapper;
using DFC.App.JobProfile.Data.Models;
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
                .ForMember(d => d.Links, s => s.Ignore());

            CreateMap<PagesApiContentItemModel, ContentItemModel>()
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published));

            CreateMap<PagesApiContentItemModel, SharedContentItemModel>()
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published));

            CreateMap<LinkDetails, PagesApiContentItemModel>()
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
