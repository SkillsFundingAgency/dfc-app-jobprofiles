using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<IEnumerable<JobProfileModel>> GetAllAsync();

        Task<JobProfileModel> GetByIdAsync(Guid documentId);

        Task<JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName);

        Task<bool> DeleteAsync(Guid documentId);
    }
}
