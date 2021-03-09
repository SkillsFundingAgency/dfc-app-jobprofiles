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
            CreateMap<HealthCheckItem, HealthItemViewModel>();

            CreateMap<ContentApiStaticElement, StaticItemCached>();

            CreateMap<IGraphItem, ContentApiBranchElement>();

            CreateMap<ContentApiRootElement, JobProfileOverview>();
            CreateMap<ContentApiRootElement, JobProfileCached>()
                .ForPath(d => d.Overview, s => s.MapFrom(model => model));

            CreateMap<ContentApiHowToBecome, JobProfileHowToBecome>();
            CreateMap<ContentApiHowToBecomeMoreInformation, HowToBecomeMoreInformation>();
            CreateMap<ApiGeneralRoute, GeneralRoute>();
            CreateMap<ApiEducationalRoute, EducationalRoute>();
            CreateMap<ApiEducationalRouteItem, EducationalRouteItem>();
            CreateMap<ApiAnchor, Anchor>();

            CreateMap<ContentApiCareerPath, JobProfileCareerPath>();
            CreateMap<ContentApiWhatItTakes, JobProfileWhatItTakes>();
            CreateMap<ContentApiWhatYoullDo, JobProfileWhatYoullDo>();

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
                .ForMember(d => d.WhatYoullDoSegment, s => s.MapFrom(x => x.WhatYoullDo));
        }
    }
}
