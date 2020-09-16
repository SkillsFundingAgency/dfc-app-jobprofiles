using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task<IList<JobProfileItemModel>?> GetSummaryListAsync();

        Task ProcessSummaryListAsync(IList<JobProfileItemModel> summaryList, CancellationToken stoppingToken);

        Task GetAndSaveItemAsync(JobProfileItemModel item, CancellationToken stoppingToken);

        Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken);

        Task DeleteStaleCacheEntriesAsync(IList<JobProfileItemModel> summaryList, CancellationToken stoppingToken);
    }
}
