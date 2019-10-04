using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Upsert Tests")]
    public class ProfileServiceUpsertTests
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly ISegmentService segmentService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceUpsertTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<IDraftJobProfileService>();
            segmentService = A.Fake<ISegmentService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService);
        }

        [Fact]
        public void JobProfileServiceUpsertReturnsSuccessWhenProfileCreated()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = jobProfileService.UpsertAsync(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceUpsertReturnsArgumentNullExceptionWhenNullParamIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.UpsertAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: jobProfileModel", exceptionResult.Message);
        }

        [Fact]
        public void JobProfileServiceUpsertReturnsNullWhenProfileNotCreated()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = A.Dummy<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = jobProfileService.UpsertAsync(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceUpsertReturnsNullWhenMissingRepository()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.UpsertAsync(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
