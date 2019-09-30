using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    public abstract class BaseProfileController
    {
        public BaseProfileController()
        {
            FakeLogger = A.Fake<ILogger<ProfileController>>();
            FakeJobProfileService = A.Fake<IJobProfileService>();
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            BrandingAssetsModel = new BrandingAssetsModel
            {
                AppCssFilePath = "no file value",
            };
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogger<ProfileController> FakeLogger { get; }

        protected IJobProfileService FakeJobProfileService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected BrandingAssetsModel BrandingAssetsModel { get; }

        protected ProfileController BuildProfileController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new ProfileController(FakeLogger, FakeJobProfileService, FakeMapper, BrandingAssetsModel)
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
