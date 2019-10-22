using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Delete Tests")]
    public class ProfileServiceDeleteTests
    {
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceDeleteTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<IDraftJobProfileService>();
            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService, mapper);
        }

        [Fact]
        public void JobProfileServiceDeleteReturnsSuccessWhenProfileDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = true;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            // act
            var result = jobProfileService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceDeleteReturnsNullWhenProfileNotDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = jobProfileService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceDeleteReturnsFalseWhenMissingRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
