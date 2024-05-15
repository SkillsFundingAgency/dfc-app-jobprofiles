using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IAVAPIService
    {
        Task<ApprenticeshipVacancyDetails> GetApprenticeshipVacancyDetailsAsync(int vacancyRef);

        Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(AVMapping mapping);

        Task<ApprenticeshipVacancySummaryResponse> GetAVSummaryPageAsync(AVMapping mapping, int pageNumber);
    }
}
