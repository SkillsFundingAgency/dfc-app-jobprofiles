using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Overview;
using DFC.App.JobProfile.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
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

            CreateMap<JobProfileModel, IndexDocumentViewModel>();

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
            //.ForMember(d => d.SalaryStarter, option => option.ConvertUsing(new SalaryToStringFormatter()))
            //.ForMember(d => d.SalaryExperienced, option => option.ConvertUsing(new SalaryToStringFormatter()))
            .ForMember(d => d.MinimumHours, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().Minimumhours))
            .ForMember(d => d.MaximumHours, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().Maximumhours))
            .ForMember(d => d.WorkingHoursDetailTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingHoursDetails.ContentItems.FirstOrDefault().DisplayText ?? string.Empty))
            .ForMember(d => d.WorkingPatternTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingPattern.ContentItems.FirstOrDefault().DisplayText ?? string.Empty))
            .ForMember(d => d.WorkingPatternDetailTitle, s => s.MapFrom(a => a.JobProfileOverview.FirstOrDefault().WorkingPatternDetails.ContentItems.FirstOrDefault().DisplayText ?? string.Empty));

            CreateMap<JobProfileVideoResponse, SocialProofVideo>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().DisplayText))
                .ForMember(d => d.Type, s => s.MapFrom(a => MapType(a.JobProfileVideo.FirstOrDefault().VideoType)))
                .ForMember(d => d.SummaryHtml, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoSummary.Html ?? string.Empty))
                .ForMember(d => d.Thumbnail, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoThumbnail))
                .ForMember(d => d.FurtherInformationHtml, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoFurtherInformation.Html ?? string.Empty))
                .ForMember(d => d.Url, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoUrl))
                .ForMember(d => d.LinkText, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoLinkText))
                .ForMember(d => d.Duration, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoDuration))
                .ForMember(d => d.Transcript, s => s.MapFrom(a => a.JobProfileVideo.FirstOrDefault().VideoTranscript));

            CreateMap<VideoThumbnail, Data.Models.Thumbnail>()
                 //find thumbnail text and replace this below - temp solution
                 .ForMember(d => d.Text, s => s.MapFrom(a => a.Paths.FirstOrDefault() ?? string.Empty))
                 .ForMember(d => d.Url, s => s.MapFrom(a => a.Urls.FirstOrDefault() ?? string.Empty));
        }

        public SocialProofVideoType MapType(string type)
        {
            if (type.ToLower() == "youtube")
            {
                return SocialProofVideoType.YouTube;
            } else
            {
                return SocialProofVideoType.Bitesize;
            }
        }
    }
}