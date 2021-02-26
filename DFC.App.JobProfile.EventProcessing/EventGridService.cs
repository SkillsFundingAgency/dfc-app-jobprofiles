// TODO: is default page for location, what's it's purpose??
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1201 // Elements should appear in the correct order
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing
{
    public class EventGridService<TModel> :
        IEventGridService<TModel>
        where TModel : class, IContentPageModel
    {
        private readonly ILogger<EventGridService<TModel>> _logger;
        private readonly IEventGridClientService _gridClient;
        private readonly EventGridPublishClientOptions _gridClientOptions;

        public EventGridService(
            ILogger<EventGridService<TModel>> logger,
            IEventGridClientService eventGridClientService,
            EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            _logger = logger;
            _gridClient = eventGridClientService;
            _gridClientOptions = eventGridPublishClientOptions;
        }

        public static bool ContainsDifferences(TModel currentModel, TModel changedModel)
        {
            _ = changedModel ?? throw new ArgumentNullException(nameof(changedModel));

            if (currentModel == null)
            {
                return true;
            }

            if (!currentModel.Equals(changedModel)) 
            {
                return true;
            }

            //if (!Equals(existingContentPageModel.IsDefaultForPageLocation, updatedContentPageModel.IsDefaultForPageLocation))
            //{
            //    return true;
            //}

            if (!Equals(currentModel.PageLocation, changedModel.PageLocation))
            {
                return true;
            }

            if (!Equals(currentModel.CanonicalName, changedModel.CanonicalName))
            {
                return true;
            }

            if ((currentModel.RedirectLocations == null && changedModel.RedirectLocations != null) ||
                (currentModel.RedirectLocations != null && changedModel.RedirectLocations == null))
            {
                return true;
            }

            if (!Enumerable.SequenceEqual(currentModel.RedirectLocations, changedModel.RedirectLocations))
            {
                return true;
            }

            return false;
        }

        public static bool IsValidEventGridPublishClientOptions(
            ILogger<EventGridService<TModel>> logger,
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

        public async Task CompareThenSendEvent(TModel currentModel, TModel changedModel)
        {
            _ = changedModel ?? throw new ArgumentNullException(nameof(changedModel));

            _logger.LogInformation($"Comparing differences to new: {changedModel.Id} - {changedModel.CanonicalName}");

            if (ContainsDifferences(currentModel, changedModel))
            {
                await SendEvent(EventOperation.CreateOrUpdate, changedModel).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"No differences to create Event Grid message for: {changedModel.Id} - {changedModel.CanonicalName}");
            }
        }

        public async Task SendEvent(EventOperation operation, TModel changedModel)
        {
            _ = changedModel ?? throw new ArgumentNullException(nameof(changedModel));

            if (!IsValidEventGridPublishClientOptions(_logger, _gridClientOptions))
            {
                _logger.LogWarning("Unable to send to event grid due to invalid EventGridPublishClientOptions options");
                return;
            }

            var logMessage = $"{operation} - {changedModel.Id} - {changedModel.CanonicalName}";
            _logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            var eventGridEvents = new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{_gridClientOptions.SubjectPrefix}{changedModel.Id}",
                    Data = new EventGridEventData
                    {
                        ItemId = changedModel.Id.ToString(),
                        Api = $"{_gridClientOptions.ApiEndpoint}{changedModel.PageLocation}",
                        DisplayText = changedModel.PageLocation,
                        VersionId = changedModel.Version.ToString(),
                        Author = _gridClientOptions.SubjectPrefix,
                    },
                    EventType = operation == EventOperation.Delete ? "deleted" : "published",
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            eventGridEvents.ForEach(Validate);

            await _gridClient.SendEvent(eventGridEvents, _gridClientOptions.TopicEndpoint, _gridClientOptions.TopicKey, logMessage).ConfigureAwait(false);
        }

        private void Validate(EventGridEvent thisEvent) => thisEvent.Validate();
    }
}
