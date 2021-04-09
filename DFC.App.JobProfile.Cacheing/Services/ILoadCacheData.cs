using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    public interface ILoadCacheData
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
