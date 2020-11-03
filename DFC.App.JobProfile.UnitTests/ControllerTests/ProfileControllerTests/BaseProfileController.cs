using AutoMapper;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    public abstract class BaseProfileController
    {
        protected BaseProfileController()
        {
            FakeLogger = A.Fake<ILogger<ProfileController>>();
            FakeJobProfileService = A.Fake<IJobProfileService>();
            FakeSharedContentService = A.Fake<ISharedContentService>();
            FakeMapper = A.Fake<IMapper>();
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

        protected ILogger<ProfileController> FakeLogger { get; }

        protected IJobProfileService FakeJobProfileService { get; }

        protected ISharedContentService FakeSharedContentService { get; }

        protected IMapper FakeMapper { get; }


        protected ProfileController BuildProfileController(
            string mediaTypeName = MediaTypeNames.Application.Json,
            IMapper mapper = null,
            string host = "localhost",
            string[] whitelist = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString(host);

            var feedbackLinks = A.Fake<FeedbackLinks>();
            var overviewDetails = A.Fake<OverviewDetails>();
            var controller = new ProfileController(FakeLogger, FakeJobProfileService, FakeSharedContentService, mapper ?? FakeMapper, feedbackLinks, overviewDetails, whitelist)
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