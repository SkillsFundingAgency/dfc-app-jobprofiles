using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<bool> Ping();

        Task<IReadOnlyCollection<JobProfileCached>> GetAllItems();

        Task<JobProfileCached> GetItemBy(Guid documentId);

        Task<JobProfileCached> GetItemBy(string canonicalName);

// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
        //Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName);
        //Task<HttpStatusCode> Create(JobProfileModel jobProfileModel);
        //Task<HttpStatusCode> Update(JobProfileModel jobProfileModel);
        //Task<HttpStatusCode> Update(JobProfileMetadata jobProfileMetadata);
        //Task<bool> DeleteAsync(Guid documentId);
    }
}