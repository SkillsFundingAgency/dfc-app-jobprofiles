using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Health Controller", "Health Tests")]
    public class HealthControllerHealthTests : BaseHealthController
    {
        [Fact]
        public async void HealthControllerHealthReturnsSuccessWhenhealthy()
        {
            // Arrange
            bool expectedResult = true;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeJobProfileService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.PingAsync()).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HealthViewModel>(jsonResult.Value);

            model.HealthItems.Count.Should().BeGreaterThan(0);
            model.HealthItems.First().Service.Should().NotBeNullOrWhiteSpace();
            model.HealthItems.First().Message.Should().NotBeNullOrWhiteSpace();

            controller.Dispose();
        }

        [Fact]
        public async void HealthControllerHealthReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            bool expectedResult = false;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeJobProfileService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async void HealthControllerHealthReturnsServiceUnavailableWhenException()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeJobProfileService.PingAsync()).Throws<Exception>();

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
