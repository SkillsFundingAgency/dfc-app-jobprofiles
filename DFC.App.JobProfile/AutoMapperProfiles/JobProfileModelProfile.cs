using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.HowToBecome;
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

            CreateMap<JobProfileHowToBecomeResponse, HowToBecomeSegmentDataModel>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().DisplayText))
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.EntryRouteSummary, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Entryroutes.Html))
                .ForPath(d => d.EntryRoutes.CommonRoutes, s => s.Ignore())
                .ForPath(d => d.EntryRoutes.Work, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Work.Html))
                .ForPath(d => d.EntryRoutes.Volunteering, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Volunteering.Html))
                .ForPath(d => d.EntryRoutes.DirectApplication, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Directapplication.Html))
                .ForPath(d => d.EntryRoutes.OtherRoutes, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Otherroutes.Html))
                .ForPath(d => d.MoreInformation.FurtherInformation, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Furtherinformation.Html))
                .ForPath(d => d.MoreInformation.CareerTips, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Careertips.Html))
                .ForPath(d => d.MoreInformation.ProfessionalAndIndustryBodies, s => s.MapFrom(a => a.JobProfileHowToBecome.FirstOrDefault().Professionalandindustrybodies.Html))
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.Registrations, s => s.Ignore())
                .ForMember(d => d.RealStory, s => s.Ignore());

            CreateMap<JobProfileHowToBecomeResponse, CommonRoutes>()
                .ForMember(d => d.RouteName, s => s.MapFrom((src, dest, routeName, context) => context.Items["RouteName"]))
                .ForMember(d => d.Subject, opt => opt.MapFrom<RelevantSubjectsResolver>())
                .ForMember(d => d.FurtherInformation, opt => opt.MapFrom<FurtherRouteInfoResolver>())
                .ForMember(d => d.EntryRequirementPreface, opt => opt.MapFrom<EntryRequirementsPrefaceResolver>())
                .ForMember(d => d.AdditionalInformation, opt => opt.MapFrom<AdditionalInfoResolver>())
                .ForMember(d => d.EntryRequirements, opt => opt.MapFrom<EntryRequirementsResolver>());
        }
    }
}