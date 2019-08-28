using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.HealthControllerTests
{
    public class BaseHealthController
    {
        public BaseHealthController()
        {
            FakeJobProfileService = A.Fake<IJobProfileService>();
            FakeLogger = A.Fake<ILogger<HealthController>>();
        }

        protected IJobProfileService FakeJobProfileService { get; }

        protected ILogger<HealthController> FakeLogger { get; }

        protected HealthController BuildHealthController()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            var controller = new HealthController(FakeLogger)
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
