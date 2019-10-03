using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Update Tests")]
    public class ProfileServiceUpdateTests
    {
        private readonly Uri dummyBaseAddressUri = new Uri("https://localhost:12345/");
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly SegmentService segmentService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceUpdateTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<DraftJobProfileService>();
            segmentService = A.Fake<SegmentService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService);
        }

        [Fact]
        public void JobProfileServiceUpdateReturnsSuccessWhenProfileReplaced()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.OK);
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.ReplaceAsync(refreshJobProfileSegmentServiceBusModel, jobProfileModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async System.Threading.Tasks.Task JobProfileServiceUpdateReturnsArgumentNullExceptionWhenNullParam1IsUsed()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.ReplaceAsync(null, jobProfileModel, dummyBaseAddressUri).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: refreshJobProfileSegmentServiceBusModel", exceptionResult.Message);
        }

        [Fact]
        public async System.Threading.Tasks.Task JobProfileServiceUpdateReturnsArgumentNullExceptionWhenNullIParam2sUsed()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.ReplaceAsync(refreshJobProfileSegmentServiceBusModel, null, dummyBaseAddressUri).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: existingJobProfileModel", exceptionResult.Message);
        }

        [Fact]
        public async System.Threading.Tasks.Task JobProfileServiceUpdateReturnsArgumentNullExceptionWhenNullIParam3sUsed()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var jobProfileModel = A.Fake<JobProfileModel>();

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.ReplaceAsync(refreshJobProfileSegmentServiceBusModel, jobProfileModel, null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: requestBaseAddress", exceptionResult.Message);
        }

        [Fact]
        public void JobProfileServiceUpdateReturnsNullWhenProfileNotReplaced()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var jobProfileModel = A.Fake<JobProfileModel>();
            var expectedResult = A.Dummy<JobProfileModel>();

            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = jobProfileService.ReplaceAsync(refreshJobProfileSegmentServiceBusModel, jobProfileModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServiceUpdateReturnsNullWhenMissingRepository()
        {
            // arrange
            var refreshJobProfileSegmentServiceBusModel = A.Fake<RefreshJobProfileSegmentServiceBusModel>();
            var jobProfileModel = A.Fake<JobProfileModel>();
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = jobProfileService.ReplaceAsync(refreshJobProfileSegmentServiceBusModel, jobProfileModel, dummyBaseAddressUri).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
