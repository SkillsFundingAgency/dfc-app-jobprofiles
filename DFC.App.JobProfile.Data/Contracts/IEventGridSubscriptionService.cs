using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IEventGridSubscriptionService
    {
        Task<HttpStatusCode> CreateAsync();

        Task<HttpStatusCode> DeleteAsync();
    }
}
