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
            CreateMap<JobProfileModel, BodyViewModel>()
                ;

            CreateMap<JobProfileModel, DocumentViewModel>()
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                .ForMember(d => d.OverviewBannerSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.OverviewBanner)))
                .ForMember(d => d.OverviewBannerSegmentUpdated, s => s.MapFrom(a => a.Data.OverviewBanner.Updated))
                .ForMember(d => d.CurrentOpportunitiesSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.CurrentOpportunities)))
                .ForMember(d => d.CurrentOpportunitiesSegmentUpdated, s => s.MapFrom(a => a.Data.CurrentOpportunities.Updated))
                .ForMember(d => d.RelatedCareersSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.RelatedCareers)))
                .ForMember(d => d.RelatedCareersSegmentUpdated, s => s.MapFrom(a => a.Data.RelatedCareers.Updated))
                .ForMember(d => d.CareerPathSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.CareerPath)))
                .ForMember(d => d.CareerPathSegmentUpdated, s => s.MapFrom(a => a.Data.CareerPath.Updated))
                .ForMember(d => d.HowToBecomeSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.HowToBecome)))
                .ForMember(d => d.HowToBecomeSegmentUpdated, s => s.MapFrom(a => a.Data.HowToBecome.Updated))
                .ForMember(d => d.WhatItTakesSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.WhatItTakes)))
                .ForMember(d => d.WhatItTakesSegmentUpdated, s => s.MapFrom(a => a.Data.WhatItTakes.Updated))
                .ForMember(d => d.WhatYouWillDoSegmentMarkup, s => s.MapFrom(a => new HtmlString(a.Markup.WhatYouWillDo)))
                .ForMember(d => d.WhatYouWillDoSegmentUpdated, s => s.MapFrom(a => a.Data.WhatYouWillDo.Updated))
                ;

            CreateMap<JobProfileModel, HeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.CssLink, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                ;

            CreateMap<JobProfileModel, IndexDocumentViewModel>()
                ;
        }
    }
}