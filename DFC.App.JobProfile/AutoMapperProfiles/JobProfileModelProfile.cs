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
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords))
                .ForMember(d => d.CareerPathSegmentContent, s => s.MapFrom(a => new HtmlString(a.Segments.CareerPath.Content)))
                .ForMember(d => d.CareerPathSegmentLastReviewed, s => s.MapFrom(a => a.Segments.CareerPath.LastReviewed))
                ;

            CreateMap<JobProfileModel, IndexDocumentViewModel>()
                ;
        }
    }
}
