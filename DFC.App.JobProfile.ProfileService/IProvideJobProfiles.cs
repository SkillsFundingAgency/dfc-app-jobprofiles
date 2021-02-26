using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IProvideJobProfiles
    {
        Task<bool> Ping();

        Task<IReadOnlyCollection<JobProfileCached>> GetAllItems();

        Task<JobProfileCached> GetItemBy(Guid documentId);

        Task<JobProfileCached> GetItemBy(string canonicalName);
    }
}