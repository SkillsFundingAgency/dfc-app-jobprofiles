using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.WebhooksControllerTests
{
    public abstract class BaseWebhooksControllerTests
    {
        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";

        protected const string LocalPath = "api/webhook";

        protected BaseWebhooksControllerTests()
        {
            Logger = A.Fake<ILogger<WebhooksController>>();
            WebhooksFakeService = A.Fake<IWebhooksService>();
        }

        protected ILogger<WebhooksController> Logger { get; }

        protected IWebhooksService WebhooksFakeService { get; }

        protected static EventGridEvent[] BuildValidEventGridEvent<TModel>(string eventType, TModel data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = "/content/sharedcontent/2c9da1b3-3529-4834-afc9-9cd741e59788",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                    Topic = "/subscriptions/962cae10-2950-412a-93e3-d8ae92b17896/resourceGroups/dfc-dev-stax-editor-rg/providers/Microsoft.EventGrid/topics/dfc-dev-stax-egt",
                },
            };

            return models;
        }

        protected static Stream BuildStreamFromModel<TModel>(TModel model)
        {
            var jsonData = JsonConvert.SerializeObject(model);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new (byteArray);

            return stream;
        }

        protected WebhooksController BuildWebhooksController(string mediaTypeName)
        {
            var objectValidator = A.Fake<IObjectModelValidator>();
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new WebhooksController(Logger, WebhooksFakeService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
                ObjectValidator = objectValidator,
            };

            A.CallTo(() => controller.ObjectValidator.Validate(A<ActionContext>.Ignored, A<ValidationStateDictionary>.Ignored, A<string>.Ignored, A<object>.Ignored));

            return controller;
        }
    }
}
