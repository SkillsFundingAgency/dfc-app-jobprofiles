using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Document Tests")]
    public class ProfileControllerDocumentTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ProfileControllerDocumentHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ProfileControllerDocumentHtmlReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            JobProfileModel expectedResult = null;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
