using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class JobProfileMetaDataPatchServiceBusModelProfile : Profile
    {
        public JobProfileMetaDataPatchServiceBusModelProfile()
        {
            CreateMap<JobProfileMetaDataPatchServiceBusModel, JobProfileModel>()
                .ForMember(d => d.DocumentId, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.MetaTags, s => s.MapFrom(a => new MetaTagsModel()))
                .ForMember(d => d.Markup, s => s.Ignore())
                .ForMember(d => d.Data, s => s.Ignore())
                ;

            CreateMap<JobProfileMetaDataPatchServiceBusModel, MetaTagsModel>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Description))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.Keywords))
                ;
        }
    }
}
