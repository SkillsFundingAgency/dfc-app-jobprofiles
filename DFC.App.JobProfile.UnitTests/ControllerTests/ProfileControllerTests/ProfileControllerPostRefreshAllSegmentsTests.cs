using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerPostRefreshAllSegmentsTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshApprenticeshipsReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = new JobProfileCurrentOpportunitiesSearchModel()
            { First = 100, Skip = 0 };
            var controller = BuildProfileController(mediaTypeName);
            int first = 100;
            int skip = 0;

            A.CallTo(() => FakeJobProfileService.RefreshAllSegments(A<string>.Ignored, first, skip)).Returns(true);

            // Act
            var result = await controller.RefreshAllSegments(refreshJobProfileSegmentModel);

            // Assert
            A.CallTo(() => FakeJobProfileService.RefreshAllSegments(A<string>.Ignored, first, skip)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<OkResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshApprenticeshipReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobProfileCurrentOpportunitiesSearchModel refreshJobProfileSegmentModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.RefreshAllSegments(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerPostRefreshApprenticeshipReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = new JobProfileCurrentOpportunitiesSearchModel();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.RefreshAllSegments(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}