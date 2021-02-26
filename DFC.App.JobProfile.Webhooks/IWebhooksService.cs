using DFC.App.JobProfile.EventProcessing.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Webhooks
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> DeleteContentItem(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentItem(Uri url, Guid contentItemId);

        Task<HttpStatusCode> ProcessMessage(EventOperation eventOperation, Guid eventId, Guid contentId, string apiEndpoint);

        //ContentApiBranchElement FindContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items);

        //bool RemoveContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items);
    }
}
