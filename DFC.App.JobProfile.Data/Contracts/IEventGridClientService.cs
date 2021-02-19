using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IEventGridClientService
    {
        Task SendEventAsync(List<EventGridEvent> eventGridEvents, string topicEndpoint, string topicKey, string logMessage);
    }
}
