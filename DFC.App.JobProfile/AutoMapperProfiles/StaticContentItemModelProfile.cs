using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class StaticContentItemModelProfile : Profile
    {
        public StaticContentItemModelProfile()
        {
            CreateMap<StaticContentItemApiDataModel, StaticContentItemModel>()
                .ForMember(d => d.Title, s => s.MapFrom(x => x.Title))
                .ForMember(d => d.Url, s => s.MapFrom(x => x.Url))
                .ForMember(d => d.Content, s => s.MapFrom(x => x.Content))
                .ForMember(d => d.Title, s => s.MapFrom(x => x.Title))
                .ForMember(d => d.Id, s => s.MapFrom(x => x.ItemId))
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore());
        }
    }
}