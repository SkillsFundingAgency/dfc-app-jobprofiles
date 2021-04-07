using AutoMapper;
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewSupport.ViewModels;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public sealed class ContentPageModelProfile :
        Profile
    {
        public ContentPageModelProfile()
        {
            // service ping models
            CreateMap<HealthCheckItem, HealthItemViewModel>();

            // current opportunity mappings, old gear to new gear
            CreateMap<SegmentVacancy, OpportunityApprenticeship>()
                .ForMember(x => x.Header, s => s.MapFrom(x => new Anchor { Link = x.URL, Text = x.Title }))
                .ForMember(x => x.Wage, s => s.MapFrom(x => x.WageText))
                .ForMember(x => x.WageFrequency, s => s.MapFrom(x => x.WageUnit))
                .ForMember(x => x.Location, s => s.MapFrom(x => x.Location.Town));
            CreateMap<SegmentCourse, OpportunityCourse>()
                .ForMember(x => x.Header, s => s.MapFrom(x => new Anchor { Link = x.URL, Text = x.Title }))
                .ForMember(x => x.Provider, s => s.MapFrom(x => x.Provider))
                .ForMember(x => x.StartDate, s => s.MapFrom(x => x.StartDate.ToString("ddd dd MMM yyyy")))
                .ForMember(x => x.Location, s => s.MapFrom(x => x.Town));
            CreateMap<SegmentCurrentOpportunity, CurrentOpportunities>()
                .ForMember(x => x.Apprenticeships, s => s.MapFrom(x => x.Data.Apprenticeship.Vacancies))
                .ForMember(x => x.Courses, s => s.MapFrom(x => x.Data.Provider.Courses));

            // content api to cache mappings
            CreateMap<ContentApiStaticElement, StaticItemCached>();

            CreateMap<IGraphItem, ContentApiBranchElement>();

            CreateMap<ContentApiJobProfile, JobProfileOverview>();
            CreateMap<ContentApiJobProfile, JobProfileCached>()
                .ForPath(d => d.Overview, s => s.MapFrom(model => model));

            CreateMap<ContentApiHowToBecome, JobProfileHowToBecome>();
            CreateMap<ContentApiHowToBecomeMoreInformation, HowToBecomeMoreInformation>();
            CreateMap<ApiGeneralRoute, GeneralRoute>();
            CreateMap<ApiEducationalRoute, EducationalRoute>();
            CreateMap<ApiEducationalRouteItem, EducationalRouteItem>();
            CreateMap<ApiAnchor, Anchor>();

            CreateMap<ContentApiCareerPath, JobProfileCareerPath>();
            CreateMap<ContentApiWhatItTakes, JobProfileWhatItTakes>();
            CreateMap<ContentApiWhatYouWillDo, JobProfileWhatYouWillDo>();

            // cache to view model mappings
            CreateMap<JobProfileCached, OccupationSummaryViewModel>()
                .ForMember(d => d.JobProfileWebsiteUrl, s => s.MapFrom(x => x.CanonicalName.Replace(" ", "-")));

            CreateMap<JobProfileCached, DocumentViewModel>()
                .ForMember(x => x.Head, opt => opt.MapFrom(model => model))
                .ForMember(x => x.HeroBanner, opt => opt.MapFrom(model => model))
                .ForMember(x => x.Body, opt => opt.MapFrom(model => model));

            CreateMap<JobProfileCached, HeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.MapFrom(x => x.PageLocation));

            CreateMap<JobProfileCached, HeroViewModel>()
                .ForMember(d => d.OverviewSegment, s => s.MapFrom(x => x.Overview));

            CreateMap<JobProfileCached, BodyViewModel>()
                .ForMember(d => d.CareerPathSegment, s => s.MapFrom(x => x.CareerPath))
                .ForMember(d => d.HowToBecomeSegment, s => s.MapFrom(x => x.HowToBecome))
                .ForMember(d => d.WhatItTakesSegment, s => s.MapFrom(x => x.WhatItTakes))
                .ForMember(d => d.WhatYouWillDoSegment, s => s.MapFrom(x => x.WhatYouWillDo));
        }
    }
}
