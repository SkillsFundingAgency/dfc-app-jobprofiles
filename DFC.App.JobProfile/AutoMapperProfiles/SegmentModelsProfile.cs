using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class SegmentModelsProfile : Profile
    {
        public SegmentModelsProfile()
        {
            CreateMap<CareerPathSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<CurrentOpportunitiesSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<HowToBecomeSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<OverviewBannerSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<RelatedCareersSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<WhatItTakesSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;

            CreateMap<WhatYouWillDoSegmentModel, DefaultSegmentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
            ;
        }
    }
}