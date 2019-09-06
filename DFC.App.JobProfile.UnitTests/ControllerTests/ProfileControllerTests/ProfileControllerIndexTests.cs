using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Index Tests")]
    public class ProfileControllerIndexTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ProfileControllerIndexHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 2;
            var expectedResults = A.CollectionOfFake<JobProfileModel>(resultsCount);
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            A.Equals(resultsCount, model.Documents.Count());

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerIndexJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 2;
            var expectedResults = A.CollectionOfFake<JobProfileModel>(resultsCount);
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(jsonResult.Value);

            A.Equals(resultsCount, model.Documents.Count());

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void PagesControllerIndexHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<JobProfileModel> expectedResults = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            A.Equals(null, model.Documents);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void PagesControllerIndexJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<JobProfileModel> expectedResults = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(jsonResult.Value);

            A.Equals(null, model.Documents);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async void PagesControllerIndexReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<JobProfileModel> expectedResults = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
