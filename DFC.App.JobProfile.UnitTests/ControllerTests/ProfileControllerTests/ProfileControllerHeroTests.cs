using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Hero Tests")]
    public class ProfileControllerHeroTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void JobProfileControllerHeroHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).Returns(A.Fake<HeroViewModel>());

            // Act
            var result = await controller.Hero(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeroViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void JobProfileControllerHeroJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Dummy<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.CanonicalName = article;
            expectedResult.Segments = new List<SegmentModel>();

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).Returns(A.Fake<HeroViewModel>());

            // Act
            var result = await controller.Hero(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<SegmentModel>>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async void JobProfileControllerHeroReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).Returns(A.Fake<HeroViewModel>());

            // Act
            var result = await controller.Hero(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobProfileModel>.Ignored, A<HeroViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
