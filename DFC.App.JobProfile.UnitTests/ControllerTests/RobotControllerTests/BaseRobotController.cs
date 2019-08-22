using DFC.App.JobProfile.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.RobotControllerTests
{
    public class BaseRobotController
    {
        public BaseRobotController()
        {
            FakeLogger = A.Fake<ILogger<RobotController>>();
            FakeHostingEnvironment = A.Fake<IHostingEnvironment>();
        }

        protected ILogger<RobotController> FakeLogger { get; }

        protected IHostingEnvironment FakeHostingEnvironment { get; }

        protected RobotController BuildRobotController()
        {
            var controller = new RobotController(FakeLogger, FakeHostingEnvironment)
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
