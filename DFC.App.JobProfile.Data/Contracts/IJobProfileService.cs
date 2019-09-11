using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<JobProfileModel>> GetAllAsync();

        Task<JobProfileModel> GetByIdAsync(Guid documentId);

        Task<JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName);

        Task<JobProfileModel> CreateAsync(CreateOrUpdateJobProfileModel createJobProfileModel);

        Task<JobProfileModel> ReplaceAsync(CreateOrUpdateJobProfileModel replaceJobProfileModel, JobProfileModel existingJobProfileModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}
