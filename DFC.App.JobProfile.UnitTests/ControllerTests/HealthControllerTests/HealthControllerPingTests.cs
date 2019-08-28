using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerPingTests : BaseHealthController
    {
        [Fact]
        public void HealthControllerPingReturnsSuccess()
        {
            // Arrange
            var controller = BuildHealthController();

            // Act
            var result = controller.Ping();

            // Assert
            var statusResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
