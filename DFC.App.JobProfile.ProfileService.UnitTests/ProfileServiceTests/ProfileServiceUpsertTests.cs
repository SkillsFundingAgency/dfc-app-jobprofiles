using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
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
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceUpsertTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();

            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper);
        }

        [Fact]
        public void JobProfileServiceUpsertReturnsSuccessWhenProfileCreated()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = jobProfileService.Create(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceUpsertReturnsArgumentNullExceptionWhenNullParamIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.Create(null).ConfigureAwait(false)).ConfigureAwait(false);

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
            var result = jobProfileService.Create(jobProfileModel).Result;

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
            Data.Models.JobProfileModel expectedResult = null;

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.Create(jobProfileModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}