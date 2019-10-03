using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
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
        private readonly Uri dummyBaseAddressUri = new Uri("https://localhost:12345/");
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
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.Created);
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.CreateAsync(refreshJobProfileSegmentServiceBusModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceCreateReturnsArgumentNullExceptionWhenNullParam1IsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.CreateAsync(null, dummyBaseAddressUri).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: refreshJobProfileSegmentServiceBusModel", exceptionResult.Message);
        }

        [Fact]
        public async Task JobProfileServiceCreateReturnsArgumentNullExceptionWhenNullParam2IsUsedAsync()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.CreateAsync(refreshJobProfileSegmentServiceBusModel, null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: requestBaseAddress", exceptionResult.Message);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsNullWhenProfileNotCreated()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var expectedResult = A.Dummy<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = jobProfileService.CreateAsync(refreshJobProfileSegmentServiceBusModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceCreateReturnsNullWhenMissingRepository()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.CreateAsync(refreshJobProfileSegmentServiceBusModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
