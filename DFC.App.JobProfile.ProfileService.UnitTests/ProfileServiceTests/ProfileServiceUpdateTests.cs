using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
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
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceUpdateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, mapper);
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