using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.WebhooksControllerTests
{
    [Trait("Category", "Webhooks Controller Unit Tests")]
    public class WebhooksControllerReceiveEventTests : BaseWebhooksControllerTests
    {
        private const HttpStatusCode ExpectedResponseOk = HttpStatusCode.OK;

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

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForCreate(string mediaTypeName, string eventType)
        {
            //Arange
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://www.ncs.com" });
            using var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.Created);

            //Act
            var result = await controller.ReceiveEvents();

            //Assert
            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)ExpectedResponseOk, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishUpdatePostReturnsOk(string mediaTypeName, string eventType)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            using var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(ExpectedResponseOk);

            // Act
            var result = await controller.ReceiveEvents();

            // Assert
            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)ExpectedResponseOk, okResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(DeletedEvents))]
        public async Task WebhooksControllerDeletePostReturnsSuccess(string mediaTypeName, string eventType)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            using var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(ExpectedResponseOk);

            // Act
            var result = await controller.ReceiveEvents();

            // Assert
            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)ExpectedResponseOk, okResult.StatusCode);
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForInvalidUrl()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { Api = "http:http://badUrl" });
            using var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents());

            // Assert
            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WebhooksControllerSubscriptionValidationReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            string expectedValidationCode = Guid.NewGuid().ToString();
            var eventGridEvents = BuildValidEventGridEvent(Microsoft.Azure.EventGrid.EventTypes.EventGridSubscriptionValidationEvent, new SubscriptionValidationEventData(expectedValidationCode, "https://somewhere.com"));
            using var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            var result = await controller.ReceiveEvents();

            // Assert
            A.CallTo(() => WebhooksFakeService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<SubscriptionValidationResponse>(jsonResult.Value);

            Assert.Equal((int)expectedResponse, jsonResult.StatusCode);
            Assert.Equal(expectedValidationCode, response.ValidationResponse);
        }
    }
}
