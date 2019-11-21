using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.MessageFunctionApp.AutoMapperProfile
{
    [ExcludeFromCodeCoverage]
    public class SitefinityMessagesMapperProfile : Profile
    {
        public SitefinityMessagesMapperProfile()
        {
            CreateMap<JobProfileMessage, JobProfileModel>()
                .ForMember(d => d.AlternativeNames, o => o.Ignore())
                .ForMember(d => d.BreadcrumbTitle, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.MetaTags, o => o.MapFrom(s => s))
                .ForMember(d => d.LastReviewed, o => o.MapFrom(s => s.LastModified))
                .ForMember(d => d.SequenceNumber, o => o.Ignore())
                ;

            CreateMap<JobProfileMessage, MetaTags>()
                 .ForMember(d => d.Description, o => o.MapFrom(s => s.Overview))
                 .ForMember(d => d.Keywords, o => o.Ignore())
                 ;
        }
    }
}