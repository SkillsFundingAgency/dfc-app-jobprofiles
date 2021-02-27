﻿// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using AutoMapper;
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<JobProfileCached, IndexDocumentViewModel>()
                .ForMember(d => d.JobProfileWebsiteUrl, s => s.MapFrom(x => x.CanonicalName.Replace(" ", "-")));

            CreateMap<JobProfileCached, HeroViewModel>()
                .ForMember(d => d.OverviewSegment, s => s.MapFrom(x => x.Overview));

            CreateMap<JobProfileCached, HeadViewModel>()
                //.ForMember(d => d.Title, s => s.MapFrom(x => x.CanonicalName))
                //.ForMember(d => d.Description, s => s.MapFrom(x => x.Description))
                //.ForMember(d => d.Keywords, s => s.MapFrom(x => x.Keywords))
                .ForMember(d => d.CanonicalUrl, s => s.MapFrom(x => x.PageLocation));

            CreateMap<ContentApiHowToBecome, JobProfileHowToBecome>();
            CreateMap<ApiEducationalRoute, EducationalRoute>();
            CreateMap<ApiEducationalRouteItem, EducationalRouteItem>();
            CreateMap<ApiAnchor, Anchor>();

            CreateMap<ContentApiHowToBecomeMoreInformation, HowToBecomeMoreInformation>();
            CreateMap<ContentApiCareerPath, JobProfileCareerPath>();
            CreateMap<ContentApiWhatItTakes, JobProfileWhatItTakes>();
            CreateMap<ContentApiWhatYoullDo, JobProfileWhatYoullDo>();
            //CreateMap<JobProfileCachedHowToBecome, ContentApiCareerPath>();
            //CreateMap<JobProfileCachedHowToBecome, ContentApiCareerPath>();
            //.ForMember(d => d.Title, s => s.MapFrom(x => x.CanonicalName))
            //.ForMember(d => d.Description, s => s.MapFrom(x => x.Description))
            //.ForMember(d => d.PageLocation, s => s.MapFrom(x => x.PageLocation))
            //.ForMember(d => d.Keywords, s => s.MapFrom(x => x.Keywords))
            //.ForMember(d => d.OverviewSegment, s => s.MapFrom(x => x.OverviewSegment));

            CreateMap<JobProfileCached, BodyViewModel>()
                .ForMember(d => d.CareerPathSegment, s => s.MapFrom(x => x.CareerPath))
                .ForMember(d => d.HowToBecomeSegment, s => s.MapFrom(x => x.HowToBecome))
                .ForMember(d => d.WhatItTakesSegment, s => s.MapFrom(x => x.WhatItTakes))
                .ForMember(d => d.WhatYoullDoSegment, s => s.MapFrom(x => x.WhatYoullDo));

            CreateMap<JobProfileCached, DocumentViewModel>()
                .ForMember(x => x.Head, opt => opt.MapFrom(model => model))
                .ForMember(x => x.HeroBanner, opt => opt.MapFrom(model => model))
                .ForMember(x => x.Body, opt => opt.MapFrom(model => model))
                .ForMember(d => d.CanonicalName, s => s.MapFrom(x => x.CanonicalName));

            CreateMap<ContentApiRootElement, JobProfileCached>()
                //.ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                //.ForMember(d => d.Links, s => s.Ignore())
                //.ForMember(d => d.ContentLinks, s => s.Ignore())
                //.ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                //.ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                //.ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))

                //.ForPath(d => d.MetaTags.Title, s => s.MapFrom(a => a.Title))
                //.ForPath(d => d.MetaTags.Description, s => s.MapFrom(a => a.Description))
                //.ForPath(d => d.MetaTags.Keywords, s => s.MapFrom(a => a.Keywords))
                .ForPath(d => d.Overview.MinimumHours, s => s.MapFrom(a => a.MinimumHours))
                .ForPath(d => d.Overview.MaximumHours, s => s.MapFrom(a => a.MaximumHours))
                .ForPath(d => d.Overview.SalaryStarter, s => s.MapFrom(a => a.SalaryStarter))
                .ForPath(d => d.Overview.SalaryExperienced, s => s.MapFrom(a => a.SalaryExperienced))
                .ForPath(d => d.Overview.WorkingPattern, s => s.MapFrom(a => a.WorkingPattern))
                .ForPath(d => d.Overview.WorkingHoursDetails, s => s.MapFrom(a => a.WorkingHoursDetails))
                .ForPath(d => d.Overview.WorkingPatternDetails, s => s.MapFrom(a => a.WorkingPatternDetails));

            CreateMap<IGraphItem, ContentApiBranchElement>();
                //.ForMember(d => d.Uri, s => s.Ignore())
                //.ForMember(d => d.Id, s => s.Ignore())
                //.ForMember(d => d.Content, s => s.Ignore())
                //.ForMember(d => d.Published, s => s.Ignore())
                //.ForMember(d => d.CreatedDate, s => s.Ignore())
                //.ForMember(d => d.Links, s => s.Ignore())
                //.ForMember(d => d.ContentLinks, s => s.Ignore())
                //.ForMember(d => d.ContentItems, s => s.Ignore());
        }
    }
}
