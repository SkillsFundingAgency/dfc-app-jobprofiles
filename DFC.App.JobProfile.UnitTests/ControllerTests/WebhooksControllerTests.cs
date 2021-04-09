using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Webhooks.Services;
using FakeItEasy;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace DFC.App.JobProfile.UnitTests.ControllerTests
{
    [ExcludeFromCodeCoverage]
    public class WebhooksControllerTests
    {
        private ILogger<WebhooksController> _mockLogger;

        private IProvideWebhooks _mockService;

        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";
        protected const string ContentTypePages = "pages";

        public static IEnumerable<object[]> PublishedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypePublished },
            new object[] { MediaTypeNames.Application.Json, EventTypeDraft },
        };

        public static IEnumerable<object[]> DeletedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypeDraftDiscarded },
            new object[] { MediaTypeNames.Application.Json, EventTypeDeleted },
            new object[] { MediaTypeNames.Application.Json, EventTypeUnpublished },
        };

        public static IEnumerable<object[]> InvalidIdValues => new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { "Not a Guid" },
        };

        protected Guid ItemIdForCreate { get; } = Guid.NewGuid();

        protected Guid ItemIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ItemIdForDelete { get; } = Guid.NewGuid();

        protected EventGridEvent[] BuildValidEventGridEvent<TModel>(string eventType, TModel data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{ContentTypePages}/a-canonical-name",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            return models;
        }

        protected Stream BuildStreamFromModel<TModel>(TModel model)
        {
            var jsonData = JsonConvert.SerializeObject(model);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        protected WebhooksController BuildWebhooksController()
        {
            _mockLogger = A.Fake<ILogger<WebhooksController>>();
            _mockService = A.Fake<IProvideWebhooks>();

            var controller = new WebhooksController(_mockLogger, _mockService);

            return controller;
        }
    }
}
