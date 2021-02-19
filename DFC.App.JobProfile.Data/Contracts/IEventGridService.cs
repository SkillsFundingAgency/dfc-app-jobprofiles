using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IEventGridService
    {
        Task CompareAndSendEventAsync(JobProfileContentPageModel existingContentPageModel, JobProfileContentPageModel updatedContentPageModel);

        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, JobProfileContentPageModel updatedContentPageModel);
    }
}
