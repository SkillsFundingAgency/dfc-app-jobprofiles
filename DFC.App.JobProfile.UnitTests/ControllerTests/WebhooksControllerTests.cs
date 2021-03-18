using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.JobProfile.Webhooks.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests
{
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

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForCreate(string eventType)
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishUpdatePostReturnsOk(string eventType)
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForUpdate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedResponse);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(DeletedEvents))]
        public async Task WebhooksControllerDeletePostReturnsSuccess(string eventType)
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForDelete.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedResponse);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForAlreadyReported(string eventType)
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForConflict(string eventType)
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.Conflict);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidIdValues))]
        public async Task WebhooksControllerPostReturnsErrorForInvalidEventId(string id)
        {
            // arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            eventGridEvents.First().Id = id;
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents());

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [MemberData(nameof(InvalidIdValues))]
        public async Task WebhooksControllerPostReturnsErrorForInvalidItemId(string id)
        {
            // arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { ItemId = id, Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents());

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForUnknownEventType()
        {
            // arrange
            var eventGridEvents = BuildValidEventGridEvent("Unknown", new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents());

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForInvalidUrl()
        {
            // arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { Api = "http:http://badUrl" });
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents());

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WebhooksControllerSubscriptionValidationReturnsSuccess()
        {
            // arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            string expectedValidationCode = Guid.NewGuid().ToString();
            var eventGridEvents = BuildValidEventGridEvent(Microsoft.Azure.EventGrid.EventTypes.EventGridSubscriptionValidationEvent, new SubscriptionValidationEventData(expectedValidationCode, "https://somewhere.com"));
            var controller = BuildWebhooksController();
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // act
            var result = await controller.ReceiveEvents();

            // assert
            A.CallTo(() => _mockService.ProcessMessage(A<EventOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<SubscriptionValidationResponse>(jsonResult.Value);

            Assert.Equal((int)expectedResponse, jsonResult.StatusCode);
            Assert.Equal(expectedValidationCode, response.ValidationResponse);
        }

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
