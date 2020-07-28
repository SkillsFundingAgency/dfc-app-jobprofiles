using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Profile Tests")]
    public class ProfileControllerProfileTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task JobProfileControllerProfileHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.DocumentId = documentId;

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Profile(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task JobProfileControllerProfileJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.DocumentId = documentId;

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Profile(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<JobProfileModel>(jsonResult.Value);

            Assert.Equal(model.DocumentId, documentId);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerDocumentHtmlAndJsonReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            Data.Models.JobProfileModel expectedResult = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Profile(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task JobProfileControllerProfileReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.DocumentId = documentId;

            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Profile(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}