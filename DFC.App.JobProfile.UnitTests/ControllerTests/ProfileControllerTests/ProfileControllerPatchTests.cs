using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Patch Tests")]
    public class ProfileControllerPatchTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPatchReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            var jobProfileMetaDataPatchModel = new JobProfileMetadata();
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.Update(A<JobProfileMetadata>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.Patch(jobProfileMetaDataPatchModel, documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.Update(jobProfileMetaDataPatchModel)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPatchWithNullParamReturnsBadRequest(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            JobProfileMetadata jobProfileMetaDataPatchModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.Patch(jobProfileMetaDataPatchModel, documentId).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}