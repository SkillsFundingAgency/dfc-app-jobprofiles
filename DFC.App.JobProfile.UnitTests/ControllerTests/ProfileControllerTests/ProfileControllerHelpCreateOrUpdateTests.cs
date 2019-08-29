using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerHelpCreateOrUpdateTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpCreateOrUpdateReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = A.Fake<CreateOrUpdateJobProfileModel>();
            JobProfileModel existingJobProfileModel = null;
            var createdJobProfileModel = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => FakeJobProfileService.CreateAsync(A<CreateOrUpdateJobProfileModel>.Ignored)).Returns(createdJobProfileModel);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.CreateAsync(A<CreateOrUpdateJobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            A.Equals((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpCreateOrUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = A.Fake<CreateOrUpdateJobProfileModel>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            JobProfileModel updatedJobProfileModel = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => FakeJobProfileService.ReplaceAsync(A<CreateOrUpdateJobProfileModel>.Ignored, A<JobProfileModel>.Ignored)).Returns(updatedJobProfileModel);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.ReplaceAsync(A<CreateOrUpdateJobProfileModel>.Ignored, A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkObjectResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpCreateOrUpdateReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            CreateOrUpdateJobProfileModel jobProfileModel = null;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpCreateOrUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var jobProfileModel = new CreateOrUpdateJobProfileModel();
            var controller = BuildProfileController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.CreateOrUpdate(jobProfileModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
