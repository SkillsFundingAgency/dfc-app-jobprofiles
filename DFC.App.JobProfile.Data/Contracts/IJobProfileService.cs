using DFC.App.JobProfile.Data.Models;
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

        Task<IEnumerable<Models.JobProfileModel>> GetAllAsync();

        Task<Models.JobProfileModel> GetByIdAsync(Guid documentId);

        Task<Models.JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<Models.JobProfileModel> GetByAlternativeNameAsync(string alternativeName);

        Task<HttpStatusCode> Create(Models.JobProfileModel jobProfileModel);

        Task<HttpStatusCode> Update(Models.JobProfileModel jobProfileModel);

        Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}