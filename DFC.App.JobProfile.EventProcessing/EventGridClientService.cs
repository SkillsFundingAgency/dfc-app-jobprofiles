using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing
{
    public class EventGridClientService :
        IEventGridClientService
    {
        private readonly ILogger<EventGridClientService> _logger;

        public EventGridClientService(ILogger<EventGridClientService> logger)
        {
            _logger = logger;
        }

        public async Task SendEvent(IReadOnlyCollection<EventGridEvent> eventGridEvents, string topicEndpoint, string topicKey, string logMessage)
        {
            _ = eventGridEvents ?? throw new ArgumentNullException(nameof(eventGridEvents));
            _ = topicEndpoint ?? throw new ArgumentNullException(nameof(topicEndpoint));
            _ = topicKey ?? throw new ArgumentNullException(nameof(topicKey));

            _logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            try
            {
                string topicHostname = new Uri(topicEndpoint).Host;
                var topicCredentials = new TopicCredentials(topicKey);
                using var client = new EventGridClient(topicCredentials);

                await client.PublishEventsAsync(topicHostname, eventGridEvents.ToList()).ConfigureAwait(false);

                _logger.LogInformation($"Sent Event Grid message for: {logMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception sending Event Grid message for: {logMessage}");
            }
        }
    }
}
