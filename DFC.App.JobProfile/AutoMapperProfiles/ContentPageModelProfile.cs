using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<JobProfileApiDataModel, ContentPageModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.PageLocation, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                //.ForMember(d => d.Links, s => s.Ignore())
                //.ForMember(d => d.ContentLinks, s => s.Ignore())
                .ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                .ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))
                .ForPath(d => d.MetaTags.Title, s => s.MapFrom(a => a.Title))
                .ForPath(d => d.MetaTags.Description, s => s.MapFrom(a => a.Description))
                .ForPath(d => d.MetaTags.Keywords, s => s.MapFrom(a => a.Keywords))
                .ForPath(d => d.OverviewSegment.MinimumHours, s => s.MapFrom(a => a.MinimumHours))
                .ForPath(d => d.OverviewSegment.MaximumHours, s => s.MapFrom(a => a.MaximumHours))
                .ForPath(d => d.OverviewSegment.SalaryStarter, s => s.MapFrom(a => a.SalaryStarter))
                .ForPath(d => d.OverviewSegment.SalaryExperienced, s => s.MapFrom(a => a.SalaryExperienced))
                .ForPath(d => d.OverviewSegment.WorkingPattern, s => s.MapFrom(a => a.WorkingPattern))
                .ForPath(d => d.OverviewSegment.WorkingHoursDetails, s => s.MapFrom(a => a.WorkingHoursDetails))
                .ForPath(d => d.OverviewSegment.WorkingPatternDetails, s => s.MapFrom(a => a.WorkingPatternDetails));

            CreateMap<LinkDetails, JobProfileApiContentItemModel>()
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
