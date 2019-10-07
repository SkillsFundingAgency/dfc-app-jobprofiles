using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class JobProfileMetaDataPatchModelProfile : Profile
    {
        public JobProfileMetaDataPatchModelProfile()
        {
            CreateMap<JobProfileMetaDataPatchModel, JobProfileModel>()
                .ForMember(d => d.DocumentId, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.SocLevelTwo, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.Markup, s => s.Ignore())
                .ForMember(d => d.Data, s => s.Ignore())
                ;
        }
    }
}
