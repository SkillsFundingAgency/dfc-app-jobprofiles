using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class SegmentsModelProfile : Profile
    {
        public SegmentsModelProfile()
        {
            CreateMap<SegmentsModel, SegmentsViewModel>()
                ;
        }
    }
}
