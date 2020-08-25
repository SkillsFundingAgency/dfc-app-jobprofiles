using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using FluentAssertions;
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

        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceCreateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, mapper);
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