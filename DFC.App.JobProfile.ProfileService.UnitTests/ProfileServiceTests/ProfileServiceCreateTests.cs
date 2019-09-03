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
    [Trait("Profile Service", "Create Tests")]
    public class ProfileServiceCreateTests
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly SegmentService segmentService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceCreateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<DraftJobProfileService>();
            segmentService = A.Fake<SegmentService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsSuccessWhenProfileCreated()
        {
            // arrange
            var createOrUdateJobProfileModel = A.Fake<CreateOrUpdateJobProfileModel>();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.CreateAsync(createOrUdateJobProfileModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceCreateReturnsArgumentNullExceptionWhenNullIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.CreateAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: createJobProfileModel", exceptionResult.Message);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsNullWhenProfileNotCreated()
        {
            // arrange
            var createOrUdateJobProfileModel = A.Fake<CreateOrUpdateJobProfileModel>();
            var expectedResult = A.Dummy<JobProfileModel>();

            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = jobProfileService.CreateAsync(createOrUdateJobProfileModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsNullWhenMissingRepository()
        {
            // arrange
            var createOrUpdateJobProfileModel = A.Fake<CreateOrUpdateJobProfileModel>();
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.CreateAsync(createOrUpdateJobProfileModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
