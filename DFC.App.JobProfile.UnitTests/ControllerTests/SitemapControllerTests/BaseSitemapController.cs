using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.SitemapControllerTests
{
    public class BaseSitemapController
    {
        public BaseSitemapController()
        {
            FakeLogger = A.Fake<ILogger<SitemapController>>();
            FakeJobProfileService = A.Fake<IJobProfileService>();
        }

        protected ILogger<SitemapController> FakeLogger { get; }

        protected IJobProfileService FakeJobProfileService { get; }

        protected SitemapController BuildSitemapController()
        {
            var controller = new SitemapController(FakeLogger, FakeJobProfileService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            return controller;
        }
    }
}
