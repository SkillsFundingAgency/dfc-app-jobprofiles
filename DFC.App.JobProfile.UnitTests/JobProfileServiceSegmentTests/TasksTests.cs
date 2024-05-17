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
    public class TasksTests
    {
        private const string CanonicalName = "auditor";
        private const string Filter = "PUBLISHED";

        [Fact]
        public async Task GetTasksValidInputAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();
            var fakeFACClient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration, fakeFACClient, fakeAVAPIService);
            var expectedResult = GetExpectedData();
            var expectedHTML = "test";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);
            A.CallTo(() => razorTemplateEngine.RenderAsync(A<string>.Ignored, A<object>.Ignored, null)).Returns(expectedHTML);

            //Act
            var response = await jobProfileService.GetTasksSegmentAsync(CanonicalName, Filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            Assert.Equal(expectedHTML, response.Markup.Value);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetTasksInvalidInputAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();
            var fakeFACClient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration, fakeFACClient, fakeAVAPIService);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfileWhatYoullDoResponse());

            //Act
            var response = await jobProfileService.GetTasksSegmentAsync(CanonicalName, Filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfileWhatYoullDoResponse GetExpectedData()
        {
            var expectedResult = new JobProfileWhatYoullDoResponse();

            var contentItemWYD = new ContentItemWYD
            {
                Description = string.Empty,
            };

            var contentItemWYDList = new List<ContentItemWYD> { contentItemWYD };

            var list = new List<JobProfileWhatYoullDo>
            {
                new JobProfileWhatYoullDo
                {
                    DisplayText = "Bookmaker",
                    Daytodaytasks = new Daytodaytasks { Html = string.Empty },
                    RelatedEnvironments = new RelatedEnvironments { ContentItems = contentItemWYDList },
                    RelatedLocations = new RelatedLocations { ContentItems = contentItemWYDList },
                    RelatedUniforms = new RelatedUniforms { ContentItems = contentItemWYDList },
                },
            };

            expectedResult.JobProfileWhatYoullDo = list;
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
