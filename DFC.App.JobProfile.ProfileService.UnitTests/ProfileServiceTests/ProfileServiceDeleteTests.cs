using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Delete Tests")]
    public class ProfileServiceDeleteTests
    {
        [Fact]
        public void JobProfileServiceDeleteReturnsSuccessWhenHelpPageDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var expectedResult = true;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            var jobProfileService = new JobProfileService(repository, A.Fake<IDraftJobProfileService>());

            // act
            var result = jobProfileService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceDeleteReturnsNullWhenHelpPageNotDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.BadRequest);

            var jobProfileService = new JobProfileService(repository, A.Fake<IDraftJobProfileService>());

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
            var repository = A.Dummy<ICosmosRepository<JobProfileModel>>();
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.FailedDependency);

            var jobProfileService = new JobProfileService(repository, A.Fake<IDraftJobProfileService>());

            // act
            var result = jobProfileService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
