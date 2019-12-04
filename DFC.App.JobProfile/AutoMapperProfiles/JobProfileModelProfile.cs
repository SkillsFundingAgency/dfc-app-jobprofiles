using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
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
            CreateMap<JobProfileModel, BodyViewModel>();
            CreateMap<JobProfileModel, HeroViewModel>();

            CreateMap<JobProfileModel, DocumentViewModel>()
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                .ForMember(
                    d => d.OverviewBannerSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.Overview)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.OverviewBannerSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.Overview)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))

                .ForMember(
                    d => d.HowToBecomeSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.HowToBecome)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.HowToBecomeSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.HowToBecome)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.WhatItTakesSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.WhatItTakes)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.WhatItTakesSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.WhatItTakes)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))

                .ForMember(
                    d => d.WhatYouWillDoSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.WhatYouWillDo)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.WhatYouWillDoSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.WhatYouWillDo)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))

                .ForMember(
                    d => d.CareerPathSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.CareerPathsAndProgression)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.CareerPathSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.CareerPathsAndProgression)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))

                .ForMember(
                    d => d.CurrentOpportunitiesSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.CurrentOpportunities)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.CurrentOpportunitiesSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.CurrentOpportunities)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))

                .ForMember(
                    d => d.RelatedCareersSegmentMarkup,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.RelatedCareers)
                            .Select(seg => seg.Markup)
                            .SingleOrDefault()))
                .ForMember(
                    d => d.RelatedCareersSegmentUpdated,
                    o => o.MapFrom(s => s.Segments
                            .Where(m => m.Segment == Data.JobProfileSegment.RelatedCareers)
                            .Select(seg => seg.RefreshedAt)
                            .SingleOrDefault()))
            ;

            CreateMap<JobProfileModel, HeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.CssLink, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title + " | Explore careers | National Careers Service"))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                ;

            CreateMap<JobProfileModel, IndexDocumentViewModel>()
                ;
        }
    }
}