using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.MessageFunctionApp.AutoMapperProfile
{
    public class SitefinityMessagesMapperProfile : Profile
    {
        public SitefinityMessagesMapperProfile()
        {
            CreateMap<JobProfileMessage, JobProfileMetadata>()
                .ForMember(d => d.AlternativeNames, o => o.Ignore())
                .ForMember(d => d.BreadcrumbTitle, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.MetaTags, o => o.MapFrom(s => s))
                .ForMember(d => d.SequenceNumber, o => o.Ignore())
                ;

            CreateMap<JobProfileMessage, MetaTags>()
                 .ForMember(d => d.Description, o => o.MapFrom(s => s.Overview))
                 .ForMember(d => d.Keywords, o => o.Ignore())
                 ;
        }
    }
}
