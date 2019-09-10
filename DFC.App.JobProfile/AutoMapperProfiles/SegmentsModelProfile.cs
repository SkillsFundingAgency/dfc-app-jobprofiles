using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class SegmentsModelProfile : Profile
    {
        public SegmentsModelProfile()
        {
            //CreateMap<SegmentsDataModel, SegmentsDataViewModel>()
            //    ;

            CreateMap<SegmentsMarkupModel, SegmentsMarkupViewModel>()
                .ForMember(d => d.OverviewBanner, s => s.MapFrom(a => new HtmlString(a.OverviewBanner)))
                .ForMember(d => d.CurrentOpportunities, s => s.MapFrom(a => new HtmlString(a.CurrentOpportunities)))
                .ForMember(d => d.RelatedCareers, s => s.MapFrom(a => new HtmlString(a.RelatedCareers)))
                .ForMember(d => d.CareerPath, s => s.MapFrom(a => new HtmlString(a.CareerPath)))
                .ForMember(d => d.HowToBecome, s => s.MapFrom(a => new HtmlString(a.HowToBecome)))
                .ForMember(d => d.WhatItTakes, s => s.MapFrom(a => new HtmlString(a.WhatItTakes)))
                .ForMember(d => d.WhatYouWillDo, s => s.MapFrom(a => new HtmlString(a.WhatYouWillDo)))
                ;
        }
    }
}
