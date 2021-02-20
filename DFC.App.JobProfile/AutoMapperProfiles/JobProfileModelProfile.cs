// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
//using AutoMapper;
//using DFC.App.JobProfile.Data.Models;
//using DFC.App.JobProfile.ViewModels;
//using System.Diagnostics.CodeAnalysis;

//namespace DFC.App.JobProfile.AutoMapperProfiles
//{
//    [ExcludeFromCodeCoverage]
//    public class JobProfileModelProfile : Profile
//    {
//        public JobProfileModelProfile()
//        {
//            CreateMap<JobProfileModel, JobProfileModel>();
//            CreateMap<JobProfileModel, BodyViewModel>()
//                .ForMember(d => d.SmartSurveyJP, s => s.Ignore());
//            CreateMap<JobProfileModel, HeroViewModel>()
//                .ForMember(d => d.Title, s => s.MapFrom(a => a.skos__prefLabel));

//            CreateMap<JobProfileModel, DocumentViewModel>()
//                .ForMember(d => d.Breadcrumb, s => s.Ignore())
//                .ForMember(d => d.Head, s => s.MapFrom(a => a))
//                .ForMember(d => d.Body, s => s.MapFrom(a => a))
//                .ForMember(d => d.Title, s => s.MapFrom(a => a.skos__prefLabel))
//                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords));

//            CreateMap<JobProfileModel, HeadViewModel>()
//                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
//                .ForMember(d => d.CssLink, s => s.Ignore())
//                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags.Title + " | Explore careers | National Careers Service"))
//                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags.Description))
//                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags.Keywords));

//            CreateMap<JobProfileModel, IndexDocumentViewModel>();
//        }
//    }
//}