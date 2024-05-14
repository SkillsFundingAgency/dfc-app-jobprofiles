using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
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
    public class OverviewTests
    {
        private const string CanonicalName = "auditor";

        [Fact]
        public async Task GetOverviewValidInputAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, fakeConfiguration, fakefacclient, fakeAVAPIService);
            var expectedResult = GetExpectedData();

            var canonicalName = "auditor";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetOverviewSegment(canonicalName, "PUBLISHED");

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetOverviewWithInvalidInputAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(mapper, logService, sharedContentRedisInterface, razorTemplateEngine, fakeConfiguration, fakefacclient, fakeAVAPIService);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfilesOverviewResponse());

            //Act
            var response = await jobProfileService.GetOverviewSegment(CanonicalName, "PUBLISHED");

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfilesOverviewResponse GetExpectedData()
        {
            var expectedResult = new JobProfilesOverviewResponse();
            var list = new List<JobProfileOverview>
            {
                new JobProfileOverview {
                    AlternativeTitle = "AlternativeTitle",
                    DisplayText = "Auditor",
                    Maximumhours = "37",
                    Minimumhours = "39",
                    WorkingHoursDetails = new WorkingHoursDetails() {ContentItems = new List<ContentItem>() {new ContentItem() {DisplayText =  "a week" } } },
                    WorkingPatternDetails = new WorkingPatternDetails() {ContentItems = new List<ContentItem>() {new ContentItem() {DisplayText = "between 8am and 6pm" } } },
                    WorkingPattern = new WorkingPattern() { ContentItems = new List<ContentItem>() {new ContentItem() {DisplayText = "working pattern" } } },
                    SalaryExperienced = "40000",
                    SalaryStarter = "30000",
                    Overview = "Overview data goes here",
                    SocCode = new SocCode()
                    {
                        ContentItems = new List<SocCodeContentItem>() { new SocCodeContentItem()
                    {
                        DisplayText = "3537",
                        SOC2020 = "2020",
                        OnetOccupationCode = "43-3031.00",
                        SOC2020extension = "2020",
                    },
                    },
                    },
                },
            };

            expectedResult.JobProfileOverview = list;
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
