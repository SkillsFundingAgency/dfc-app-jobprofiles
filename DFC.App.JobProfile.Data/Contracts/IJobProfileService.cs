using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<bool> PingAsync();

        Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync();

        Task<IEnumerable<JobProfileModel>> GetAllAsync();

        Task<JobProfileModel> GetByIdAsync(Guid documentId);

        Task<JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName);

        Task<JobProfileModel> CreateAsync(RefreshJobProfileSegmentServiceBusModel refreshJobProfileSegmentServiceBusModel, Uri requestBaseAddress);

        Task<JobProfileModel> ReplaceAsync(RefreshJobProfileSegmentServiceBusModel refreshJobProfileSegmentServiceBusModel, JobProfileModel existingJobProfileModel, Uri requestBaseAddress);

        Task<bool> DeleteAsync(Guid documentId);
    }
}
