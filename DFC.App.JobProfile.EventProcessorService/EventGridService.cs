using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ClientOptions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessorService
{
    public class EventGridService :
        IEventGridService
    {
        private readonly ILogger<EventGridService> _logger;
        private readonly IEventGridClientService _gridClient;
        private readonly EventGridPublishClientOptions _gridClientOptions;

        public EventGridService(
            ILogger<EventGridService> logger,
            IEventGridClientService eventGridClientService,
            EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            _logger = logger;
            _gridClient = eventGridClientService;
            _gridClientOptions = eventGridPublishClientOptions;
        }

        public static bool ContainsDifferences<TModel>(
            TModel existingContentPageModel,
            TModel updatedContentPageModel)
                where TModel : IJobProfileCached
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            if (existingContentPageModel == null)
            {
                return true;
            }

            if (!existingContentPageModel.Equals(updatedContentPageModel)) 
            {
                return true;
            }

            if (!Equals(existingContentPageModel.IsDefaultForPageLocation, updatedContentPageModel.IsDefaultForPageLocation))
            {
                return true;
            }

            if (!Equals(existingContentPageModel.PageLocation, updatedContentPageModel.PageLocation))
            {
                return true;
            }

            if (!Equals(existingContentPageModel.CanonicalName, updatedContentPageModel.CanonicalName))
            {
                return true;
            }

            if ((existingContentPageModel.RedirectLocations == null && updatedContentPageModel.RedirectLocations != null) ||
                (existingContentPageModel.RedirectLocations != null && updatedContentPageModel.RedirectLocations == null))
            {
                return true;
            }

            if (!Enumerable.SequenceEqual(existingContentPageModel.RedirectLocations, updatedContentPageModel.RedirectLocations))
            {
                return true;
            }

            return false;
        }

        public static bool IsValidEventGridPublishClientOptions(
            ILogger<EventGridService> logger,
            EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            _ = eventGridPublishClientOptions ?? throw new ArgumentNullException(nameof(eventGridPublishClientOptions));

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.SubjectPrefix))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.SubjectPrefix)}");
                return false;
            }

            if (eventGridPublishClientOptions.ApiEndpoint == null)
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.ApiEndpoint)}");
                return false;
            }

            return true;
        }

        public async Task CompareAndSendEventAsync(
            IJobProfileCached existingContentPageModel,
            IJobProfileCached updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            _logger.LogInformation($"Comparing differences to new: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");

            if (ContainsDifferences(existingContentPageModel, updatedContentPageModel))
            {
                await SendEventAsync(WebhookCacheOperation.CreateOrUpdate, updatedContentPageModel).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"No differences to create Event Grid message for: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");
            }
        }

        public async Task SendEventAsync(
            WebhookCacheOperation webhookCacheOperation,
            IJobProfileCached updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            if (!IsValidEventGridPublishClientOptions(_logger, _gridClientOptions))
            {
                _logger.LogWarning("Unable to send to event grid due to invalid EventGridPublishClientOptions options");
                return;
            }

            var logMessage = $"{webhookCacheOperation} - {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}";
            _logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            var eventGridEvents = new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{_gridClientOptions.SubjectPrefix}{updatedContentPageModel.Id}",
                    Data = new EventGridEventData
                    {
                        ItemId = updatedContentPageModel.Id.ToString(),
                        Api = $"{_gridClientOptions.ApiEndpoint}{updatedContentPageModel.PageLocation}",
                        DisplayText = updatedContentPageModel.PageLocation,
                        VersionId = updatedContentPageModel.Version.ToString(),
                        Author = _gridClientOptions.SubjectPrefix,
                    },
                    EventType = webhookCacheOperation == WebhookCacheOperation.Delete ? "deleted" : "published",
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            eventGridEvents.ForEach(f => f.Validate());

            await _gridClient.SendEventAsync(eventGridEvents, _gridClientOptions.TopicEndpoint, _gridClientOptions.TopicKey, logMessage).ConfigureAwait(false);
        }
    }
}
