﻿using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobProfileMetaDataPatchModelProfile : Profile
    {
        public JobProfileMetaDataPatchModelProfile()
        {
            CreateMap<JobProfileMetadata, JobProfileModel>()
                .ForMember(d => d.DocumentId, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.SocLevelTwo, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.Segments, s => s.Ignore())
                ;
        }
    }
}
