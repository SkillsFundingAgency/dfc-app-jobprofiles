using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IRefreshService
    {
        Task<HttpStatusCode> RefreshCoursesAsync(int retryCount = 0);

        Task<HttpStatusCode> RefreshAllSegmentsAsync(int retryCount = 0);
    }
}
