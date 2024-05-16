using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.JobProfileServiceSegmentTests
{
    public class SocialProofVideoTests
    {
        [Fact]
        public async Task GetSocialProofVideoValidInputAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration, fakefacclient, fakeAVAPIService);
            var expectedResult = GetExpectedData();

            var canonicalName = "bookmaker";
            var filter = "PUBLISHED";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetSocialProofVideoSegment(canonicalName, filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SocialProofVideo));
        }

        [Fact]
        public async Task GetHowToBecomeInvalidInputAsync()
        {
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration, fakefacclient, fakeAVAPIService);
            var canonicalName = "bookmaker";
            var filter = "PUBLISHED";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfileVideoResponse());

            //Act
            var response = await jobProfileService.GetSocialProofVideoSegment(canonicalName, filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeNull();
        }

        private static JobProfileVideoResponse GetExpectedData()
        {
            var expectedResult = new JobProfileVideoResponse();

            var jobProfileVideo = new JobProfileVideo
            {
                DisplayText = string.Empty,
                VideoType = string.Empty,
                VideoTitle = string.Empty,
                VideoTranscript = string.Empty,
                VideoSummary = new VideoSummary { Html = string.Empty },
                VideoThumbnail = new VideoThumbnail { MediaText = new List<string>() { string.Empty }, Urls = new List<string>() { string.Empty } },
                VideoUrl = string.Empty,
                VideoLinkText = string.Empty,
                VideoFurtherInformation = new VideoFurtherInformation { Html = string.Empty },
                VideoDuration = string.Empty,
            };

            expectedResult = new JobProfileVideoResponse { JobProfileVideo = new List<JobProfileVideo>() { jobProfileVideo } };

            return expectedResult;
        }

        private IMapper GetMapperInstance()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new JobProfileModelProfile());
            });
            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}
