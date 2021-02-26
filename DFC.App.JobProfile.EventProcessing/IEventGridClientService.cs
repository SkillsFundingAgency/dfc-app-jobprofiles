using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing
{
    public interface IEventGridClientService
    {
        Task SendEvent(IReadOnlyCollection<EventGridEvent> eventGridEvents, string topicEndpoint, string topicKey, string logMessage);
    }
}
