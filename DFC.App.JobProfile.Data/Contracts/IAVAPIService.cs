using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IAVAPIService
    {
        Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(AVMapping mapping);
    }
}
