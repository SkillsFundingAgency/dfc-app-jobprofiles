using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using NHibernate.Engine;
using Razor.Templating.Core;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.JobProfileServiceSegmentTests
{
    public class CareerPathTests
    {
        [Fact]
        public async void GetRelatedCareersDataSuccess()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeCourseSearch = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var canonicalName = "biochemist";
            var status = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakeCourseSearch, fakeAVAPIService);
            var expectedResult = GetExpectedData();

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetCareerPathSegmentAsync(canonicalName, status);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetRelatedCareersDataNoSuccessAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeCourseSearch = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var canonicalName = "biochemist";
            var status = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakeCourseSearch, fakeAVAPIService);

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfileCareerPathAndProgressionResponse());

            //Act
            var response = await jobProfileService.GetCareerPathSegmentAsync(canonicalName, status);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfileCareerPathAndProgressionResponse GetExpectedData()
        {
            var expectedResult = new JobProfileCareerPathAndProgressionResponse();

            var list = new List<JobProfileCareerPathAndProgression>
            {
                new JobProfileCareerPathAndProgression
                {
                    DisplayText = "biochemist",
                    Content = new JobProileCareerPath()
                    {
                        Html = "ExampleTestHtmlStringBiochemist",
                    },
                },
            };

            expectedResult.JobProileCareerPath = list;
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
