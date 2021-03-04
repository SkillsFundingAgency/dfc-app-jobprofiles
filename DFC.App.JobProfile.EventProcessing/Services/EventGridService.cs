// TODO: is default page for location, what's it's purpose??
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1201 // Elements should appear in the correct order
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing.Services
{
    internal sealed class EventGridService<TModel> :
        IEventGridService<TModel>,
        IRequireServiceRegistration
        where TModel : class, IContentPageModel
    {
        private readonly ILogger<EventGridService<TModel>> _logger;
        private readonly IEventGridClientService _gridClient;
        private readonly IEventGridPublicationConfiguration _gridConfiguration;

        public EventGridService(
            ILogger<EventGridService<TModel>> logger,
            IEventGridClientService eventGridClientService,
            IEventGridPublicationConfiguration publicationConfiguration)
        {
            _logger = logger;
            _gridClient = eventGridClientService;
            _gridConfiguration = publicationConfiguration;

            //(!IsValidConfiguration())
            //    .AsGuard<FormatException>("Event grid publication configuration invalid");
        }

        public async Task CompareThenSendEvent(TModel currentModel, TModel changedModel)
        {
            _ = changedModel ?? throw new ArgumentNullException(nameof(changedModel));

            _logger.LogInformation($"Comparing differences to new: {changedModel.Id} - {changedModel.CanonicalName}");

            if (ContainsDifferences(currentModel, changedModel))
            {
                await SendEvent(EventOperation.CreateOrUpdate, changedModel);
            }
            else
            {
                _logger.LogInformation($"No differences to create Event Grid message for: {changedModel.Id} - {changedModel.CanonicalName}");
            }
        }

        public async Task SendEvent(EventOperation operation, TModel changedModel)
        {
            if (!IsValidConfiguration())
            {
                return;
            }

            _ = changedModel ?? throw new ArgumentNullException(nameof(changedModel));

            var logMessage = $"{operation} - {changedModel.Id} - {changedModel.CanonicalName}";
            _logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            var eventGridEvents = new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{_gridConfiguration.SubjectPrefix}{changedModel.Id}",
                    Data = new EventGridEventData
                    {
                        ItemId = changedModel.Id.ToString(),
                        Api = $"{_gridConfiguration.ApiEndpoint}{changedModel.PageLocation}",
                        DisplayText = changedModel.PageLocation,
                        VersionId = changedModel.Version.ToString(),
                        Author = _gridConfiguration.SubjectPrefix,
                    },
                    EventType = operation == EventOperation.Delete ? "deleted" : "published",
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            eventGridEvents.ForEach(Validate);

            await _gridClient.SendEvent(eventGridEvents, _gridConfiguration.TopicEndpoint, _gridConfiguration.TopicKey, logMessage);
        }

        internal bool ContainsDifferences(TModel currentModel, TModel changedModel)
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

        internal bool IsValidConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_gridConfiguration.SubjectPrefix))
            {
                _logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(_gridConfiguration.SubjectPrefix)}");
                return false;
            }

            if (_gridConfiguration.ApiEndpoint == null)
            {
                _logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(_gridConfiguration.ApiEndpoint)}");
                return false;
            }

            return true;
        }

        internal void Validate(EventGridEvent thisEvent) => thisEvent.Validate();
    }
}
