using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerPutUpdateTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPutUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.Update(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.Update(refreshJobProfileSegmentModel);

            // Assert
            A.CallTo(() => FakeJobProfileService.Update(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPutUpdatepReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobProfileModel refreshJobProfileSegmentModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.Update(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPutUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = new JobProfileModel();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Update(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}