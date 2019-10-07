using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using System;
using System.Collections.Generic;
using System.Net;
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

        Task<HttpStatusCode> UpsertAsync(JobProfileModel jobProfileModel);

        Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegmentModel refreshJobProfileSegmentModel, JobProfileModel existingJobProfileModel, Uri requestBaseAddress);

        Task<bool> DeleteAsync(Guid documentId);
    }
}
