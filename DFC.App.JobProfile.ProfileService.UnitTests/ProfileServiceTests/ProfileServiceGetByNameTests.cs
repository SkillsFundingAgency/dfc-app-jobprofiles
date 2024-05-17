using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetByName Tests")]
    public class ProfileServiceGetByNameTests
    {
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine fakeRazorTemplateEngine;
        private readonly IConfiguration fakeConfiguration;
        private readonly IAVAPIService fakeAVAPIService;
        private readonly ICourseSearchApiService fakeFACClient;

        public ProfileServiceGetByNameTests()
        {
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            fakeConfiguration = A.Fake<IConfiguration>();
            fakeAVAPIService = A.Fake<IAVAPIService>();
            fakeFACClient = A.Fake<ICourseSearchApiService>();
            jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeFACClient, fakeAVAPIService);
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsSuccess()
        {
            // arrange
            var fakeJobProfileService = A.Fake<IJobProfileService>();

            var expectedResult = A.Fake<JobProfileModel>();
            expectedResult.Segments = new List<SegmentModel>
            {
                new SegmentModel { Segment = JobProfileSegment.Overview },
                new SegmentModel { Segment = JobProfileSegment.CurrentOpportunities },
                new SegmentModel { Segment = JobProfileSegment.RelatedCareers },
                new SegmentModel { Segment = JobProfileSegment.HowToBecome },
                new SegmentModel { Segment = JobProfileSegment.CareerPathsAndProgression },
                new SegmentModel { Segment = JobProfileSegment.WhatItTakes },
                new SegmentModel { Segment = JobProfileSegment.WhatYouWillDo },
            };

            var allEnumValues = Enum.GetValues(typeof(JobProfileSegment)).Cast<JobProfileSegment>();
            var distinctSegments = expectedResult.Segments.Select(s => s.Segment).Distinct();

            A.CallTo(() => fakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(new JobProfilesOverviewResponse()
                {
                    JobProfileOverview = new List<JobProfileOverview> { new JobProfileOverview() },
                });

            // act
            var result = await jobProfileService.GetByNameAsync("auditor").ConfigureAwait(false);

            // assert
            Assert.Equal(result.Segments.Count, expectedResult.Segments.Count);
            Assert.Equal(allEnumValues.Count(), distinctSegments.Count());
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'canonicalName')", exceptionResult.Message);
        }
    }
}