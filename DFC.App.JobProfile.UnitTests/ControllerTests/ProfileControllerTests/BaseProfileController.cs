using AutoMapper;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Models;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    public abstract class BaseProfileController
    {
        protected BaseProfileController()
        {
            FakeLogger = A.Fake<ILogService>();
            FakeJobProfileService = A.Fake<IJobProfileService>();
            FakeMapper = A.Fake<IMapper>();
            FakeSegmentService = A.Fake<ISegmentService>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<string[]>
        {
            new[] { "*/*" },
            new[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<string[]>
        {
            new[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<string[]>
        {
            new[] { MediaTypeNames.Application.Json },
        };

        protected ILogService FakeLogger { get; }

        protected IJobProfileService FakeJobProfileService { get; }

        protected IMapper FakeMapper { get; }

        protected ISegmentService FakeSegmentService { get; }

        protected ProfileController BuildProfileController(string mediaTypeName = MediaTypeNames.Application.Json, IMapper mapper = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var feedbackLinks = A.Fake<FeedbackLinks>();
            var controller = new ProfileController(FakeLogger, FakeJobProfileService, mapper ?? FakeMapper, feedbackLinks, FakeSegmentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}