using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ILoadJobProfileContent :
        ILoadCacheData
    {
        //Task<IList<JobProfileSummaryModel>?> GetSummaryListAsync();

        //Task ProcessSummaryListAsync(IList<JobProfileSummaryModel> summaryList, CancellationToken stoppingToken);

        //Task GetAndSaveItemAsync(JobProfileSummaryModel item, CancellationToken stoppingToken);

        //Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken);

        //Task DeleteStaleCacheEntriesAsync(IList<JobProfileSummaryModel> summaryList, CancellationToken stoppingToken);
    }
}
