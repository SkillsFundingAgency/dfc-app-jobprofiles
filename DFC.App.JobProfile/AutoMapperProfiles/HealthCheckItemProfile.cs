using AutoMapper;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class HealthCheckItemProfile : Profile
    {
        public HealthCheckItemProfile()
        {
            CreateMap<HealthCheckItem, HealthItemViewModel>();
        }
    }
}
