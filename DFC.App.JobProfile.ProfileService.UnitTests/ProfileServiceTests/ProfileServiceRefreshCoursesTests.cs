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
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var mapper = A.Fake<IMapper>();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakeclient = A.Fake<ICourseSearchApiService>();
            var expectedResult = GetExpectedData();
            var status = "PUBLISHED";

            var jobProfileService = new JobProfileService(repository, A.Fake<SegmentService>(), mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeclient);

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            // act
            var response = await jobProfileService.RefreshCourses(status);

            // assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Equals(true);
        }

        [Fact]
        public async Task JobProfileServiceRefreshCoursesReturnsArgumentNullExceptionWhenNullParamIsUsedAsync()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var expectedResult = true;
            var mapper = A.Fake<IMapper>();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakeclient = A.Fake<ICourseSearchApiService>();
            var jobProfileService = new JobProfileService(repository, A.Fake<SegmentService>(), mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeclient);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshCourses(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            exceptionResult.Should().BeOfType(typeof(ArgumentNullException));
        }

        private static JobProfileCurrentOpportunitiesResponse GetExpectedData()
        {
            var expectedResult = new JobProfileCurrentOpportunitiesResponse();
            var list = new List<JobProfileCurrentOpportunities>
            {
                new JobProfileCurrentOpportunities {
                    DisplayText = "Auditor",
                    Coursekeywords = "'building services engineering'",
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
