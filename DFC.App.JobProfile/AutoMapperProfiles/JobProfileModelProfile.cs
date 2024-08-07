﻿using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers;
using DFC.App.JobProfile.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Data.Models.Segment.Tasks;
using DFC.App.JobProfile.ProfileService.Models;
using DFC.App.JobProfile.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DFC.App.JobProfile.Data.Models.Segment.CareerPath;
using DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities;
using DFC.App.JobProfile.Data.Models.Segment.Overview;
using DFC.App.JobProfile.Data.Models.Segment.RelatedCareers;
using DFC.App.JobProfile.Data.Models.Segment.SkillsModels;
using JobProfSkills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Skills;
using RelatedSkill = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.RelatedSkill;
using Skills = DFC.App.JobProfile.Data.Models.Segment.SkillsModels.Skills;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobProfileModelProfile : Profile
    {
        public JobProfileModelProfile()
        {
            CreateMap<JobProfileModel, IndexDocumentViewModel>();

            CreateMap<JobProfileModel, ViewModels.BodyViewModel>()
                .ForMember(d => d.SmartSurveyJP, s => s.Ignore())
                .ForMember(d => d.SpeakToAnAdviser, s => s.Ignore());

            CreateMap<JobProfileModel, HeroViewModel>()
                .ForMember(d => d.ShowLmi, s => s.Ignore())
                .ForMember(d => d.LmiLink, s => s.MapFrom(a => a));

            CreateMap<JobProfileModel, LmiLinkViewModel>()
                .ForMember(d => d.CanonicalName, s => s.MapFrom(a => a.CanonicalName))
                .ForMember(d => d.OverviewSegmentModel, s => s.MapFrom(a =>
                    JsonConvert.DeserializeObject<OverviewSegmentModel>(a.Segments.Any(x => x.Segment == JobProfileSegment.Overview) ?
                    a.Segments.FirstOrDefault(x => x.Segment == JobProfileSegment.Overview).JsonV1 : string.Empty)));

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

            CreateMap<RelatedCareersResponse, RelatedCareerSegmentDataModel>()
                .ForMember(d => d.RelatedCareers, s => s.MapFrom<RelatedCareerResolver>())
                .ForMember(d => d.LastReviewed, s => s.Ignore());

            CreateMap<JobProfileWhatYoullDoResponse, TasksSegmentDataModel>()
                .ForMember(d => d.Tasks, s => s.MapFrom(a => a.JobProfileWhatYoullDo.FirstOrDefault().Daytodaytasks.Html))
                .ForMember(d => d.Location, s => s.MapFrom<LocationResolver>())
                .ForMember(d => d.Environment, s => s.MapFrom<EnvironmentResolver>())
                .ForMember(d => d.Uniform, s => s.MapFrom<UniformResolver>())
                .ForMember(d => d.Introduction, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore());

            CreateMap<JobProfileHowToBecomeResponse, HowToBecomeSegmentDataModel>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().DisplayText))
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.EntryRouteSummary, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().EntryRoutes.Html))
                .ForPath(d => d.EntryRoutes.CommonRoutes, s => s.Ignore())
                .ForPath(d => d.EntryRoutes.Work, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Work.Html))
                .ForPath(d => d.EntryRoutes.Volunteering, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Volunteering.Html))
                .ForPath(d => d.EntryRoutes.DirectApplication, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().DirectApplication.Html))
                .ForPath(d => d.EntryRoutes.OtherRoutes, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().OtherRoutes.Html))
                .ForPath(d => d.MoreInformation.FurtherInformation, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().FurtherInformation.Html))
                .ForPath(d => d.MoreInformation.CareerTips, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().CareerTips.Html))
                .ForPath(d => d.MoreInformation.ProfessionalAndIndustryBodies, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().ProfessionalAndIndustryBodies.Html))
                .ForMember(d => d.Registrations, s => s.MapFrom<RegistrationResolver>())
                .ForMember(d => d.RealStory, s => s.MapFrom<RealStoryResolver>());

            CreateMap<JobProfileHowToBecomeResponse, CommonRoutes>()
                .ForMember(d => d.RouteName, s => s.MapFrom((src, dest, routeName, context) => context.Items["RouteName"]))
                .ForMember(d => d.Subject, s => s.MapFrom<RelevantSubjectsResolver>())
                .ForMember(d => d.FurtherInformation, s => s.MapFrom<FurtherRouteInfoResolver>())
                .ForMember(d => d.EntryRequirementPreface, s => s.MapFrom<EntryRequirementsPrefaceResolver>())
                .ForMember(d => d.AdditionalInformation, s => s.MapFrom<AdditionalInfoResolver>())
                .ForMember(d => d.EntryRequirements, s => s.MapFrom<EntryRequirementsResolver>());

            CreateMap<JobProfilesOverviewResponse, OverviewApiModel>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().DisplayText))
                .ForMember(d => d.Breadcrumb, option => option.Ignore())
                .ForMember(d => d.LastUpdatedDate, option => option.Ignore())
                .ForMember(d => d.Url, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().PageLocation.UrlName))
                .ForMember(d => d.Soc, option => option.ConvertUsing(new SocCodeFormatter(), a => a.JobProfileOverview.FirstOrDefault().SocCode.ContentItems.FirstOrDefault().DisplayText))
                .ForMember(d => d.Soc2020, option => option.ConvertUsing(new SocCodeFormatter(), a => a.JobProfileOverview.FirstOrDefault().SocCode.ContentItems.FirstOrDefault().SOC2020))
                .ForMember(d => d.Soc2020Extension, option => option.ConvertUsing(new SocCodeFormatter(), a => a.JobProfileOverview.FirstOrDefault().SocCode.ContentItems.FirstOrDefault().SOC2020extension))
                .ForMember(d => d.ONetOccupationalCode, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().SocCode.ContentItems.FirstOrDefault().OnetOccupationCode))
                .ForMember(d => d.AlternativeTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().AlternativeTitle))
                .ForMember(d => d.Overview, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().Overview))
                .ForMember(d => d.SalaryStarter, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().SalaryStarter))
                .ForMember(d => d.SalaryExperienced, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().SalaryExperienced))
                .ForMember(d => d.MinimumHours, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().Minimumhours))
                .ForMember(d => d.MaximumHours, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().Maximumhours))
                .ForMember(d => d.WorkingHoursDetailTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingHoursDetails.ContentItems.FirstOrDefault().DisplayText ?? string.Empty))
                .ForMember(d => d.WorkingPatternTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingPattern.ContentItems.FirstOrDefault().DisplayText ?? string.Empty))
                .ForMember(d => d.WorkingPatternDetailTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingPatternDetails.ContentItems.FirstOrDefault().DisplayText ?? string.Empty));

            CreateMap<ApprenticeshipVacancySummary, Vacancy>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.WageUnit, s => s.MapFrom(a => a.Wage.WageUnit))
                .ForMember(d => d.WageText, s => s.MapFrom(a => a.Wage.WageAdditionalInformation))
                .ForMember(d => d.Location, s => s.MapFrom<VacancyLocationResolver>())
                .ForMember(d => d.URL, s => s.MapFrom(a => a.VacancyUrl))
                .ForMember(d => d.PullDate, s => s.Ignore());

            CreateMap<JobProfileCareerPathAndProgressionResponse, CareerPathSegmentDataModel>()
                .ForMember(d => d.Markup, s => s.MapFrom(a => a.JobProileCareerPath.FirstOrDefault().Content.Html))
                .ForMember(d => d.LastReviewed, d => d.Ignore());

            CreateMap<JobProfileSkillsResponse, JobProfileSkillSegmentDataModel>()
                .ForMember(d => d.DigitalSkill, s => s.MapFrom(a => a.JobProfileSkills.FirstOrDefault().DigitalSkills.ContentItems.FirstOrDefault().DisplayText))
                .ForMember(d => d.OtherRequirements, s => s.MapFrom(a => a.JobProfileSkills.FirstOrDefault().Otherrequirements.Html))
                .ForMember(d => d.Restrictions, s => s.MapFrom<SkillsResolver>())
                .ForMember(d => d.Skills, d => d.Ignore())
                .ForMember(d => d.LastReviewed, d => d.Ignore());

            CreateMap<JobSkills, Skills>()
                .ForMember(d => d.OnetSkill, s => s.MapFrom(a => a.Skills))
                .ForMember(d => d.ContextualisedSkill, s => s.MapFrom(a => a.JobProfileContextualisedSkills));

            CreateMap<JobProfSkills, OnetSkill>()
                .ForMember(d => d.ONetElementId, s => s.MapFrom(a => a.ONetElementId))
                .ForMember(d => d.Title, s => s.MapFrom(a => a.DisplayText))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Description))
                .ForMember(d => d.Id, s => s.MapFrom(a => a.GraphSync.NodeId.Substring(a.GraphSync.NodeId.LastIndexOf('/') + 1)));

            CreateMap<RelatedSkill, ContextualisedSkill>()
                .ForMember(d => d.ONetRank, s => s.MapFrom(a => a.ONetRank))
                .ForMember(d => d.ONetAttributeType, s => s.MapFrom(a => a.ONetAttributeType))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.RelatedSkillDesc))
                .ForMember(d => d.Id, s => s.MapFrom(a => a.GraphSync.NodeId.Substring(a.GraphSync.NodeId.LastIndexOf('/') + 1)))
                .ForMember(d => d.OriginalRank, d => d.Ignore());

            CreateMap<Course, Opportunity>()
                .ForMember(d => d.Provider, s => s.MapFrom(f => f.ProviderName))
                .ForMember(d => d.PullDate, s => s.Ignore())
                .ForMember(d => d.Url, s => s.Ignore())
                .ForPath(d => d.Location.Town, s => s.MapFrom(f => f.Location));

            CreateMap<JobProfileVideoResponse, SocialProofVideo>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoTitle))
                .ForMember(d => d.Type, s => s.MapFrom(a => MapType(a.JobProfileVideo.FirstOrDefault().VideoType)))
                .ForMember(d => d.SummaryHtml, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoSummary.Html ?? string.Empty))
                .ForMember(d => d.Thumbnail, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoThumbnail))
                .ForMember(d => d.FurtherInformationHtml, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoFurtherInformation.Html ?? string.Empty))
                .ForMember(d => d.Url, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoUrl))
                .ForMember(d => d.LinkText, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoLinkText))
                .ForMember(d => d.Duration, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoDuration))
                .ForMember(d => d.Transcript, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoTranscript));

            CreateMap<VideoThumbnail, Data.Models.Thumbnail>()
                 .ForMember(d => d.Text, s => s.MapFrom(a => a.MediaText.FirstOrDefault() ?? string.Empty))
                 .ForMember(d => d.Url, s => s.MapFrom(a => a.Urls.FirstOrDefault() ?? string.Empty));

            CreateMap<JobProfileCurrentOpportunities, JobProfileModel>()
                .ForMember(d => d.CanonicalName, opt => opt.MapFrom(s => s.PageLocation.UrlName))
                .ForMember(d => d.IncludeInSitemap, opt => opt.MapFrom(s => true))
                .ForMember(d => d.BreadcrumbTitle, opt => opt.MapFrom(s => s.DisplayText));
        }

        public SocialProofVideoType MapType(string type)
        {
            if (type != null && type.ToLower() == "youtube")
            {
                return SocialProofVideoType.YouTube;
            } else
            {
                return SocialProofVideoType.Bitesize;
            }
        }
    }
}