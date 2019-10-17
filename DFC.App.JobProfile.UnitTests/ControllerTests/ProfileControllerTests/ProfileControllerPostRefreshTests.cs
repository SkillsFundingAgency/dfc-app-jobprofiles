﻿using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerPostRefreshTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPostRefreshReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            Data.Models.JobProfileModel existingRefreshJobProfileSegment = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingRefreshJobProfileSegment);
            A.CallTo(() => FakeJobProfileService.RefreshSegmentsAsync(A<RefreshJobProfileSegment>.Ignored, A<JobProfileModel>.Ignored, A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.PostRefresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.RefreshSegmentsAsync(A<RefreshJobProfileSegment>.Ignored, A<JobProfileModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            var noContentResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, noContentResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPostRefreshReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var existingRefreshJobProfileSegment = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingRefreshJobProfileSegment);

            // Act
            var result = await controller.PostRefresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.RefreshSegmentsAsync(A<RefreshJobProfileSegment>.Ignored, A<JobProfileModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPostRefreshReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            RefreshJobProfileSegment refreshJobProfileSegmentModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.PostRefresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ProfileControllerPostRefreshReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.PostRefresh(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
