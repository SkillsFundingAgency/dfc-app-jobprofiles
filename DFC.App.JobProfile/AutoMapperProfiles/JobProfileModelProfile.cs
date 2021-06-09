using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobProfileModelProfile : Profile
    {
        public JobProfileModelProfile()
        {
            CreateMap<JobProfileModel, JobProfileModel>();
            CreateMap<JobProfileModel, BodyViewModel>()
                .ForMember(d => d.SmartSurveyJP, s => s.Ignore());

            CreateMap<JobProfileModel, HeroViewModel>()
                .ForMember(d => d.ShowLmi, s => s.Ignore())
                .ForMember(d => d.LmiLink, s => s.MapFrom(a => a));

            CreateMap<JobProfileModel, LmiLinkViewModel>()
                .ForMember(d => d.CanonicalName, s => s.MapFrom(a => a.CanonicalName))
                .ForMember(d => d.OverviewSegmentModel, s => s.MapFrom(a => JsonConvert.DeserializeObject<OverviewSegmentModel>(a.Segments.Any(x => x.Segment == JobProfileSegment.Overview) ? a.Segments.FirstOrDefault(x => x.Segment == JobProfileSegment.Overview).JsonV1 : string.Empty)));

            CreateMap<JobProfileModel, DocumentViewModel>()
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Head, s => s.MapFrom(a => a))
                .ForMember(d => d.Body, s => s.MapFrom(a => a))
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords));

            CreateMap<JobProfileModel, HeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.CssLink, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title + " | Explore careers | National Careers Service"))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords));

            CreateMap<JobProfileModel, IndexDocumentViewModel>();
        }
    }
}