using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.Webhooks;
using DFC.App.JobProfile.Webhooks.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    [Route("api/webhook")]
    public class WebhooksController : Controller
    {
        private readonly Dictionary<string, EventOperation> acceptedEventTypes = new Dictionary<string, EventOperation>
        {
            ["draft"] = EventOperation.CreateOrUpdate,
            ["published"] = EventOperation.CreateOrUpdate,
            ["draft-discarded"] = EventOperation.Delete,
            ["unpublished"] = EventOperation.Delete,
            ["deleted"] = EventOperation.Delete,
        };

        private readonly ILogger<WebhooksController> _logger;
        private readonly IProvideWebhooks _webhooks;

        public WebhooksController(
            ILogger<WebhooksController> logger,
            IProvideWebhooks webhooks)
        {
            _logger = logger;
            _webhooks = webhooks;
        }

        [HttpPost]
        [Route("ReceiveEvents")]
        public async Task<IActionResult> ReceiveEvents()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string requestContent = await reader.ReadToEndAsync();
            _logger.LogInformation($"Received events: {requestContent}");

            var eventGridSubscriber = new EventGridSubscriber();
            foreach (var key in acceptedEventTypes.Keys)
            {
                eventGridSubscriber.AddOrUpdateCustomEventMapping(key, typeof(EventGridEventData));
            }

            var eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                if (!Guid.TryParse(eventGridEvent.Id, out Guid eventId))
                {
                    throw new InvalidDataException($"Invalid Guid for EventGridEvent.Id '{eventGridEvent.Id}'");
                }

                if (eventGridEvent.Data is SubscriptionValidationEventData subscriptionValidationEventData)
                {
                    _logger.LogInformation($"Got SubscriptionValidation event data, validationCode: {subscriptionValidationEventData!.ValidationCode},  validationUrl: {subscriptionValidationEventData.ValidationUrl}, topic: {eventGridEvent.Topic}");

                    // Do any additional validation (as required) such as validating that the Azure resource ID of the topic matches
                    // the expected topic and then return back the below response
                    var responseData = new SubscriptionValidationResponse()
                    {
                        ValidationResponse = subscriptionValidationEventData.ValidationCode,
                    };

                    return Ok(responseData);
                }
                else if (eventGridEvent.Data is EventGridEventData eventGridEventData)
                {
                    if (!Guid.TryParse(eventGridEventData.ItemId, out Guid contentId))
                    {
                        throw new InvalidDataException($"Invalid Guid for EventGridEvent.Data.ItemId '{eventGridEventData.ItemId}'");
                    }

                    if (!Uri.TryCreate(eventGridEventData.Api, UriKind.Absolute, out Uri url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{eventGridEventData.Api}' received for Event Id: {eventId}");
                    }

                    var cacheOperation = acceptedEventTypes[eventGridEvent.EventType];

                    _logger.LogInformation($"Got Event Id: {eventId}: {eventGridEvent.EventType}: Cache operation: {cacheOperation} {url}");

                    var result = await _webhooks.ProcessMessage(cacheOperation, eventId, contentId, eventGridEventData.Api);

                    LogResult(eventId, contentId, result);
                }
                else
                {
                    throw new InvalidDataException($"Invalid event type '{eventGridEvent.EventType}' received for Event Id: {eventId}, should be one of '{string.Join(",", acceptedEventTypes.Keys)}'");
                }
            }

            return Ok();
        }

        private void LogResult(Guid eventId, Guid contentPageId, HttpStatusCode result)
        {
            switch (result)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"Event Id: {eventId}, Content Page Id: {contentPageId}: Updated Content Page");
                    break;

                case HttpStatusCode.Created:
                    _logger.LogInformation($"Event Id: {eventId}, Content Page Id: {contentPageId}: Created Content Page");
                    break;

                case HttpStatusCode.AlreadyReported:
                    _logger.LogInformation($"Event Id: {eventId}, Content Page Id: {contentPageId}: Content Page previously updated");
                    break;

                default:
                    _logger.LogWarning($"Event Id: {eventId}, Content Page Id: {contentPageId}: Content Page not Posted: Status: {result}");
                    break;
            }
        }
    }
}