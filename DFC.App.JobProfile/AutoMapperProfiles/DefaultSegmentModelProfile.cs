using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class DefaultSegmentModelProfile : Profile
    {
        public DefaultSegmentModelProfile()
        {
            CreateMap<DefaultSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;
        }
    }
}