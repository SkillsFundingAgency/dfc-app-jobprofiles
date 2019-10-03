using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerCreateOrUpdateTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerCreateOrUpdateReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            JobProfileModel existingRefreshJobProfileSegment = null;
            var createdJobProfileModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingRefreshJobProfileSegment);
            A.CallTo(() => FakeJobProfileService.CreateAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored, A<Uri>.Ignored)).Returns(createdJobProfileModel);

            // Act
            var result = await controller.CreateOrUpdate(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.CreateAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            A.Equals((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerCreateOrUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var existingRefreshJobProfileSegment = A.Fake<JobProfileModel>();
            JobProfileModel updatedJobProfileModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingRefreshJobProfileSegment);
            A.CallTo(() => FakeJobProfileService.ReplaceAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored, A<JobProfileModel>.Ignored, A<Uri>.Ignored)).Returns(updatedJobProfileModel);

            // Act
            var result = await controller.CreateOrUpdate(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.ReplaceAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored, A<JobProfileModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkObjectResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerCreateOrUpdateReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            RefreshJobProfileSegmentServiceBusModel refreshJobProfileSegmentServiceBusModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.CreateOrUpdate(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerCreateOrUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentServiceBusModel = new RefreshJobProfileSegmentServiceBusModel();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.CreateOrUpdate(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
