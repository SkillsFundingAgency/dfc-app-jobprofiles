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
        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);

        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentAsync(Uri url, Guid contentId);

        Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId);

        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint);

        JobProfileApiContentItemModel? FindContentItem(Guid contentItemId, IList<JobProfileApiContentItemModel> items);

        bool RemoveContentItem(Guid contentItemId, IList<JobProfileApiContentItemModel> items);
    }
}
