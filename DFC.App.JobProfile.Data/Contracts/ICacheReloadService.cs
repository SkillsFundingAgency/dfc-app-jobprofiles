using DFC.App.JobProfile.Data.Models;
using dfc_content_pkg_netcore.models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task<IList<PagesSummaryItemModel>?> GetSummaryListAsync();

        Task ProcessSummaryListAsync(IList<PagesSummaryItemModel> summaryList, CancellationToken stoppingToken);

        Task GetAndSaveItemAsync(PagesSummaryItemModel item, CancellationToken stoppingToken);

        Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken);

        Task DeleteStaleCacheEntriesAsync(IList<PagesSummaryItemModel> summaryList, CancellationToken stoppingToken);
    }
}
