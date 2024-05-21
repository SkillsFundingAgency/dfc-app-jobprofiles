using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Refresh Courses Tests")]
    public class ProfileServiceRefreshCoursesTests
    {
        [Fact]
        public async Task JobProfileServiceRefreshCoursesReturnsSuccessAsync()
        {
            // arrange
            var mapper = A.Fake<IMapper>();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakeclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var expectedResult = GetExpectedData();
            var status = "PUBLISHED";
            int first = 100;
            int skip = 0;

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeclient, fakeAVAPIService);

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiryAndFirstSkip<JobProfileCurrentOpportunitiesResponse>(A<string>.Ignored, A<string>.Ignored, first, skip, A<double>.Ignored)).Returns(expectedResult);

            // act
            var response = await jobProfileService.RefreshCourses(status, first, skip);

            // assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiryAndFirstSkip<JobProfileCurrentOpportunitiesResponse>(A<string>.Ignored, A<string>.Ignored, first, skip, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsType<bool>(response);
            Assert.True(response);
        }

        [Fact]
        public async Task JobProfileServiceRefreshCoursesReturnsArgumentNullExceptionWhenNullParamIsUsedAsync()
        {
            // arrange
            var mapper = A.Fake<IMapper>();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakeclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(repository, A.Fake<SegmentService>(), mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeclient, fakeAVAPIService);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshCourses(null, 100, 0).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            exceptionResult.Should().BeOfType(typeof(ArgumentNullException));
        }

        private static JobProfileCurrentOpportunitiesResponse GetExpectedData()
        {
            var expectedResult = new JobProfileCurrentOpportunitiesResponse();
            var list = new List<JobProfileCurrentOpportunities>
            {
                new JobProfileCurrentOpportunities
                {
                    DisplayText = "Auditor",
                    Coursekeywords = "'building services engineering'",
                    PageLocation = new Common.SharedContent.Pkg.Netcore.Model.Common.PageLocation()
                    {
                        UrlName = "Auditor",
                    },
                    SOCCode = new SOCCode()
                    {
                        ContentItems = new SOCCodeContentItem[]
                        {
                            new SOCCodeContentItem()
                            {
                                ApprenticeshipStandards = new ApprenticeshipStandards()
                                {
                                    ContentItems= new ApprenticeshipStandardsContentItem[]
                                    {
                                        new ApprenticeshipStandardsContentItem()
                                        {
                                            LARScode = "123456",
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            expectedResult.JobProfileCurrentOpportunities = list;
            return expectedResult;
        }
    }
}
