using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.AutoMapperProfiles
{
    public class HealthCheckItemProfile : Profile
    {
        public HealthCheckItemProfile()
        {
            CreateMap<HealthCheckItem, HealthItemViewModel>()
                ;
        }
    }
}
