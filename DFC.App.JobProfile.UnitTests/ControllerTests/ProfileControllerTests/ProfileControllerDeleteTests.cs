using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Delete Tests")]
    public class ProfileControllerDeleteTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpDeleteReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerHelpDeleteReturnsNotFound(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            JobProfileModel expectedResult = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
