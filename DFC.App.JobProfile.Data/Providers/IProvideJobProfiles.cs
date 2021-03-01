using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    public interface IProvideJobProfiles :
        IProvidePageContent<JobProfileCached>
    {
        Task<IReadOnlyCollection<JobProfileCached>> GetAllItems();
    }
}