using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Ping / Health Tests")]
    public class ProfileServicePingTests
    {
        private IMapper mapper;

        [Fact]
        public void JobProfileServicePingReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var expectedResult = true;
            mapper = A.Fake<IMapper>();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var jobProfileService = new JobProfileService(repository, A.Fake<SegmentService>(), mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, fakeConfiguration);

            // act
            var result = jobProfileService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<JobProfileModel>>();
            var expectedResult = false;
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var logService = A.Fake<ILogService>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var jobProfileService = new JobProfileService(repository, A.Fake<SegmentService>(), mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, fakeConfiguration);

            // act
            var result = jobProfileService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}