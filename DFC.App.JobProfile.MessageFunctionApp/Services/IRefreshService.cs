using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IRefreshService
    {
        Task<HttpStatusCode> RefreshApprenticeshipsAsync(int first, int skip, int retryCount = 0);

        Task<HttpStatusCode> RefreshCoursesAsync(int first, int skip, int retryCount = 0);

        Task<HttpStatusCode> RefreshAllSegmentsAsync(int first, int skip, int retryCount = 0);

        Task<int> CountJobProfiles(int retryCount = 0);
    }
}
