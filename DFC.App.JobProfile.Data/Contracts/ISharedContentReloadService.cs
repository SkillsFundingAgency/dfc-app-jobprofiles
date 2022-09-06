using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ISharedContentReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
