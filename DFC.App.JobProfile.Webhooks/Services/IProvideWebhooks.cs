using DFC.App.JobProfile.EventProcessing.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Webhooks.Services
{
    public interface IProvideWebhooks
    {
        Task<HttpStatusCode> DeleteContentItem(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentItem(Uri url, Guid contentItemId);

        Task<HttpStatusCode> ProcessMessage(EventOperation eventOperation, Guid eventId, Guid contentId, string apiEndpoint);
    }
}
