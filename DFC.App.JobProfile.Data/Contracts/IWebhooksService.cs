using System;
using System.Net;
using System.Threading.Tasks;

using DFC.App.JobProfile.Data.Enums;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint);
    }
}
