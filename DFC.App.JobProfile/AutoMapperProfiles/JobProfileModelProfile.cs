using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class JobProfileModelProfile : Profile
    {
        public JobProfileModelProfile()
        {
            CreateMap<JobProfileModel, DocumentViewModel>()
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                .ForMember(d => d.OverviewBannerSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.OverviewBanner.Content)))
                .ForMember(d => d.OverviewBannerSegmentLastReviewed, s => s.MapFrom(a => a.Segments.OverviewBanner.LastReviewed))
                .ForMember(d => d.CurrentOpportunitiesSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.CurrentOpportunities.Content)))
                .ForMember(d => d.CurrentOpportunitiesSegmentLastReviewed, s => s.MapFrom(a => a.Segments.CurrentOpportunities.LastReviewed))
                .ForMember(d => d.RelatedCareersSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.RelatedCareers.Content)))
                .ForMember(d => d.RelatedCareersSegmentLastReviewed, s => s.MapFrom(a => a.Segments.RelatedCareers.LastReviewed))
                .ForMember(d => d.CareerPathSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.CareerPath.Content)))
                .ForMember(d => d.CareerPathSegmentLastReviewed, s => s.MapFrom(a => a.Segments.CareerPath.LastReviewed))
                .ForMember(d => d.HowToBecomeSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.HowToBecome.Content)))
                .ForMember(d => d.HowToBecomeSegmentLastReviewed, s => s.MapFrom(a => a.Segments.HowToBecome.LastReviewed))
                .ForMember(d => d.WhatItTakesSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.WhatItTakes.Content)))
                .ForMember(d => d.WhatItTakesSegmentLastReviewed, s => s.MapFrom(a => a.Segments.WhatItTakes.LastReviewed))
                ;

            CreateMap<JobProfileModel, IndexDocumentViewModel>()
                ;
        }
    }
}