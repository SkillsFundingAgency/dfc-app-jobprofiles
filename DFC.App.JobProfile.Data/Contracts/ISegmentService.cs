using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ISegmentService
    {
        Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync();

        Task<SegmentModel> RefreshSegmentAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel);
    }
}