using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers;
using DFC.App.JobProfile.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CareerPath;
using DFC.App.JobProfile.Data.Models.Overview;
using DFC.App.JobProfile.Data.Models.RelatedCareersModels;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
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

            CreateMap<RelatedCareerDataModel, RelatedCareerDataViewModel>();

            CreateMap<JobProfileModel, IndexDocumentViewModel>();

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

            CreateMap<JobProfileCareerPathAndProgressionResponse, CareerPathSegmentDataModel>()
                .ForMember(d => d.Markup, s => s.MapFrom(a => a.JobProileCareerPath.FirstOrDefault().Content.Html))
                .ForMember(d => d.LastReviewed, d => d.Ignore());
        }
    }
}