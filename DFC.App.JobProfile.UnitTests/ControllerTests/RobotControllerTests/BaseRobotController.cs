using DFC.App.JobProfile.Controllers;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.RobotControllerTests
{
    public class BaseRobotController
    {
        public BaseRobotController()
        {
            FakeLogger = A.Fake<ILogService>();
            FakeHostingEnvironment = A.Fake<IHostingEnvironment>();
        }

        protected ILogService FakeLogger { get; }

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
