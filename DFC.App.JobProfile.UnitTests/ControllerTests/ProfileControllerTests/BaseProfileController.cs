﻿using AutoMapper;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            DummyConfigValues = A.Dummy<ConfigValues>();
            FakeSegmentService = A.Fake<ISegmentService>();
            FakeRedirectionSecurityService = A.Fake<IRedirectionSecurityService>();
            FakeStaticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            FakeConfiguration = A.Fake<IConfiguration>();
            FakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
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

        protected ConfigValues DummyConfigValues { get; }

        protected ISegmentService FakeSegmentService { get; }

        protected IRedirectionSecurityService FakeRedirectionSecurityService { get; }

        protected IDocumentService<StaticContentItemModel> FakeStaticContentDocumentService { get; }

        protected IConfiguration FakeConfiguration { get; }

        protected ISharedContentRedisInterface FakeSharedContentRedisInterface { get; }


        protected ProfileController BuildProfileController(
            string mediaTypeName = MediaTypeNames.Application.Json,
            IMapper mapper = null,
            string host = "localhost",
            string[] whitelist = null,
            string sharedContentItemId = "2c9da1b3-3529-4834-afc9-9cd741e59788")
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString(host);

            var feedbackLinks = A.Fake<FeedbackLinks>();
            var cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = sharedContentItemId,
            };

            var controller = new ProfileController(FakeLogger, FakeJobProfileService, mapper ?? FakeMapper, DummyConfigValues, feedbackLinks, FakeSegmentService, FakeRedirectionSecurityService, FakeStaticContentDocumentService, cmsApiClientOptions, FakeSharedContentRedisInterface, FakeConfiguration)
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