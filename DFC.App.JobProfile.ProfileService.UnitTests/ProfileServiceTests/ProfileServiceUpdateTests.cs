using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Razor.Templating.Core;
using System;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Create Tests")]
    public class ProfileServiceUpdateTests
    {
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;

        public ProfileServiceUpdateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();

            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine);
        }

        [Fact]
        public void JobProfileServiceUpdateReturnsSuccessWhenProfileUpdated()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.Update(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(jobProfileModel, existingJobProfileModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceUpdateReturnsNotFoundWhenNoProfileFoundForUpdate()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            JobProfileModel existingJobProfileModel = null;
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);

            // act
            var result = jobProfileService.Update(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(jobProfileModel, existingJobProfileModel)).MustNotHaveHappened();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServicePatchUpdateReturnsSuccessWhenProfileUpdated()
        {
            // arrange
            var jobProfileMetadata = A.Fake<JobProfileMetadata>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.Update(jobProfileMetadata).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(jobProfileMetadata, existingJobProfileModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServicePatchUpdateReturnsNotFoundWhenNoProfileFoundForUpdate()
        {
            // arrange
            var jobProfileMetadata = A.Fake<JobProfileMetadata>();
            JobProfileModel existingJobProfileModel = null;
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);

            // act
            var result = jobProfileService.Update(jobProfileMetadata).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(jobProfileMetadata, existingJobProfileModel)).MustNotHaveHappened();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}