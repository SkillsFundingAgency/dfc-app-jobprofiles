using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IEventGridService
    {
        Task CompareAndSendEventAsync(ContentPageModel? existingContentPageModel, ContentPageModel? updatedContentPageModel);

        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, ContentPageModel? updatedContentPageModel);
    }
}
