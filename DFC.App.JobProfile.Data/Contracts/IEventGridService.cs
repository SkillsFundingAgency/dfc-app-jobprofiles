using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IEventGridService
    {
        Task CompareAndSendEventAsync(IJobProfileCached existingContentPageModel, IJobProfileCached updatedContentPageModel);

        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, IJobProfileCached updatedContentPageModel);
    }
}
