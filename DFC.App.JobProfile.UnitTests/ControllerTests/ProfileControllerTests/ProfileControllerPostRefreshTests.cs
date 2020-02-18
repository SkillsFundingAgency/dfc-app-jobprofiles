using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerPostRefreshTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var controller = BuildProfileController(mediaTypeName);
            A.CallTo(() => FakeJobProfileService.RefreshSegmentsAsync(A<RefreshJobProfileSegment>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.Refresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.RefreshSegmentsAsync(A<RefreshJobProfileSegment>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            RefreshJobProfileSegment refreshJobProfileSegmentModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.Refresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Refresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}