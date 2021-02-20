using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId);

        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint);

        ContentApiBranchElement FindContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items);

        bool RemoveContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items);
    }
}
