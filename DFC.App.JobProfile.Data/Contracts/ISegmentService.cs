using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ISegmentService
    {
        CreateOrUpdateJobProfileModel CreateOrUpdateJobProfileModel { get; set; }

        JobProfileModel JobProfileModel { get; set; }

        Uri RequestBaseAddress { get; set; }

        Task LoadAsync();

        Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync();
    }
}