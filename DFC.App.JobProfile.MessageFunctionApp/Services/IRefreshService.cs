using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IRefreshService
    {
        Task<HttpStatusCode> RefreshApprenticeshipsAsync(int retryCount = 0);

        Task<HttpStatusCode> RefreshCoursesAsync(int retryCount = 0);
    }
}
