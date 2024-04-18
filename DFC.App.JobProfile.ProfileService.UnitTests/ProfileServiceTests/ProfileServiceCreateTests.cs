using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Razor.Templating.Core;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Create Tests")]
    public class ProfileServiceCreateTests
    {
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;

        public ProfileServiceCreateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();

            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsSuccessWhenProfileCreated()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = HttpStatusCode.OK;
            JobProfileModel nullModel = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(nullModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = jobProfileService.Create(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsAlreadyReportedWhenProfileExists()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = HttpStatusCode.AlreadyReported;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(jobProfileModel);

            // act
            var result = jobProfileService.Create(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceCreateReturnsArgumentNullExceptionWhenNullParamIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.Create(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            exceptionResult.Should().BeOfType(typeof(ArgumentNullException));
        }
    }
}