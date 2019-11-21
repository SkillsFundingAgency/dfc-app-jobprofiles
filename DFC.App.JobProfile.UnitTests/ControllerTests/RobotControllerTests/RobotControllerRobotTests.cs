using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.RobotControllerTests
{
    [Trait("Category", "Robot Controller Unit Tests")]
    public class RobotControllerRobotTests : BaseRobotController
    {
        [Fact]
        public void RobotControllerRobotReturnsSuccess()
        {
            // Arrange
            var controller = BuildRobotController();

            // Act
            var result = controller.Robot();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);

            contentResult.ContentType.Should().Be(MediaTypeNames.Text.Plain);

            controller.Dispose();
        }
    }
}
