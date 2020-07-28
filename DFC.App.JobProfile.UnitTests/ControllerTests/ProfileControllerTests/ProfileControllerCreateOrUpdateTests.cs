using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerCreateOrUpdateTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerCreateOrUpdateReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.Create(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await controller.Create(jobProfileModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.Create(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.Created, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerCreateOrUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.Create(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.Create(jobProfileModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.Create(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerCreateOrUpdateReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobProfileModel jobProfileModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.Create(jobProfileModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerCreateOrUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = new JobProfileModel();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Create(jobProfileModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}