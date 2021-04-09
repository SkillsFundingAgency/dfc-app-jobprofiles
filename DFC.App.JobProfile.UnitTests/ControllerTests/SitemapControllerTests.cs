//using DFC.App.JobProfile.Controllers;
//using DFC.App.JobProfile.Data.Models;
//using DFC.App.JobProfile.Data.Providers;
//using FakeItEasy;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Net.Mime;
//using System.Threading.Tasks;
//using Xunit;

//namespace DFC.App.JobProfile.UnitTests.ControllerTests
//{
//    public class SitemapControllerTests
//    {
//        private ILogger<SitemapController> _mockLogger;

//        private IProvideJobProfiles _mockService;

//        [Fact]
//        public async Task SitemapControllerSitemapReturnsSuccess()
//        {
//            // Arrange
//            const int resultsCount = 3;
//            var expectedResults = A.CollectionOfFake<JobProfileCached>(resultsCount);
//            var controller = BuildSitemapController();

//            expectedResults[0].CanonicalName = "default-article-in-sitemap";
//            expectedResults[1].CanonicalName = "not-in-sitemap";
//            expectedResults[2].CanonicalName = "in-sitemap";

//            A.CallTo(() => _mockService.GetAllItems()).Returns(expectedResults);

//            // Act
//            var result = await controller.Sitemap();

//            // Assert
//            A.CallTo(() => _mockService.GetAllItems()).MustHaveHappenedOnceExactly();

//            var contentResult = Assert.IsType<ContentResult>(result);

//            contentResult.ContentType.Should().Be(MediaTypeNames.Application.Xml);
//        }

//        [Fact]
//        public async Task SitemapControllerSitemapReturnsSuccessWhenNoData()
//        {
//            // arrange
//            const int resultsCount = 0;
//            var expectedResults = A.CollectionOfFake<JobProfileCached>(resultsCount);
//            var controller = BuildSitemapController();

//            A.CallTo(() => _mockService.GetAllItems()).Returns(expectedResults);

//            // act
//            var result = await controller.Sitemap();

//            // assert
//            A.CallTo(() => _mockService.GetAllItems()).MustHaveHappenedOnceExactly();

//            var contentResult = Assert.IsType<ContentResult>(result);

//            contentResult.ContentType.Should().Be(MediaTypeNames.Application.Xml);
//        }

//        protected SitemapController BuildSitemapController()
//        {
//            _mockLogger = A.Fake<ILogger<SitemapController>>();
//            _mockService = A.Fake<IProvideJobProfiles>();

//            return new SitemapController(_mockLogger, _mockService);
//        }
//    }
//}
