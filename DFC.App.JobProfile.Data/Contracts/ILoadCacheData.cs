using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ILoadCacheData
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
