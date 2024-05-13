using DFC.App.JobProfile.MessageFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IRefreshService
    {
        Task<HttpStatusCode> RefreshApprenticeshipsAsync(Guid documentId);

        Task<HttpStatusCode> RefreshCoursesAsync(int retryCount = 0);

        Task<IList<SimpleJobProfileModel>> GetListAsync();
    }
}
