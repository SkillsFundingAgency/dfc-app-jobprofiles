using DFC.App.JobProfile.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests
{
    [ExcludeFromCodeCoverage]
    public class RobotControllerTests
    {
        private ILogger<RobotController> _mockLogger;

        private IHostEnvironment _mockEnvironment;

        [Fact]
        public void RobotControllerRobotReturnsSuccess()
        {
            // arrange
            var controller = BuildRobotController();

            // act
            var result = controller.Robot();

            // assert
            var contentResult = Assert.IsType<ContentResult>(result);
            contentResult.ContentType.Should().Be(MediaTypeNames.Text.Plain);
        }

        protected RobotController BuildRobotController()
        {
            _mockLogger = A.Fake<ILogger<RobotController>>();
            _mockEnvironment = A.Fake<IHostEnvironment>();

            return new RobotController(_mockLogger, _mockEnvironment);
        }
    }
}
