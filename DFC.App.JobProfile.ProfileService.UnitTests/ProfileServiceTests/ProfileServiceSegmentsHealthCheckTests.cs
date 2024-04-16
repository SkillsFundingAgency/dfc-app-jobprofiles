﻿using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Segments Health Check Tests")]
    public class ProfileServiceSegmentsHealthCheckTests
    {
        [Fact]
        public void JobProfileServiceSegmentsHealthCheckReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var segmentService = A.Fake<ISegmentService>();
            var mapper = A.Fake<IMapper>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var logService = A.Fake<ILogService>();

            IList<HealthCheckItem> expectedResult = new List<HealthCheckItem>
            {
                new HealthCheckItem
                {
                    Service = "Unit test",
                    Message = "All ok",
                },
            };

            A.CallTo(() => segmentService.SegmentsHealthCheckAsync()).Returns(expectedResult);

            var jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, fakeSharedContentRedisInterface);

            // act
            var result = jobProfileService.SegmentsHealthCheckAsync().Result;

            // assert
            A.CallTo(() => segmentService.SegmentsHealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}