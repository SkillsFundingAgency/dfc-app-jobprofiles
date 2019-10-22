//using AutoMapper;
//using DFC.App.JobProfile.Data.Contracts;
//using DFC.App.JobProfile.Data.Models;
//using DFC.App.JobProfile.Data.Models.ServiceBusModels;
//using DFC.App.JobProfile.DraftProfileService;
//using FakeItEasy;
//using System;
//using System.Net;
//using Xunit;

//namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
//{
//    [Trait("Profile Service", "RefreshSegments Tests")]
//    public class ProfileServiceRefreshSegmentsTests
//    {
//        private readonly Uri dummyBaseAddressUri = new Uri("https://localhost:12345/");
//        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;
//        private readonly IDraftJobProfileService draftJobProfileService;
//        private readonly ISegmentService segmentService;
//        private readonly IMapper mapper;
//        private readonly IJobProfileService jobProfileService;

//        public ProfileServiceRefreshSegmentsTests()
//        {
//            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
//            draftJobProfileService = A.Fake<IDraftJobProfileService>();
//            segmentService = A.Fake<ISegmentService>();
//            mapper = A.Fake<IMapper>();
//            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService, mapper);
//        }

//        //[Fact]
//        //public void JobProfileServiceRefreshSegmentsReturnsSuccessWhenProfileReplaced()
//        //{
//        //    // arrange
//        //    var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
//        //    var jobProfileModel = A.Fake<JobProfileModel>();
//        //    var expectedResult = A.Fake<JobProfileModel>();

//        //    A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.OK);

//        //    // act
//        //    var result = jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, jobProfileModel, dummyBaseAddressUri).Result;

//        //    // assert
//        //    A.CallTo(() => segmentService.LoadAsync()).MustHaveHappenedOnceExactly();
//        //    A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
//        //    A.Equals(result, expectedResult);
//        //}

//        [Fact]
//        public async System.Threading.Tasks.Task JobProfileServiceRefreshSegmentsReturnsArgumentNullExceptionWhenNullParam1IsUsed()
//        {
//            // arrange
//            var jobProfileModel = A.Fake<JobProfileModel>();

//            // act
//            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshSegmentsAsync(null, jobProfileModel, dummyBaseAddressUri).ConfigureAwait(false)).ConfigureAwait(false);

//            // assert
//            Assert.Equal("Value cannot be null.\r\nParameter name: refreshJobProfileSegmentModel", exceptionResult.Message);
//        }

//        [Fact]
//        public async System.Threading.Tasks.Task JobProfileServiceRefreshSegmentsReturnsArgumentNullExceptionWhenNullIParam2sUsed()
//        {
//            // arrange
//            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();

//            // act
//            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, null, dummyBaseAddressUri).ConfigureAwait(false)).ConfigureAwait(false);

//            // assert
//            Assert.Equal("Value cannot be null.\r\nParameter name: existingJobProfileModel", exceptionResult.Message);
//        }

//        [Fact]
//        public async System.Threading.Tasks.Task JobProfileServiceRefreshSegmentsReturnsArgumentNullExceptionWhenNullIParam3sUsed()
//        {
//            // arrange
//            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
//            var jobProfileModel = A.Fake<JobProfileModel>();

//            // act
//            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, jobProfileModel, null).ConfigureAwait(false)).ConfigureAwait(false);

//            // assert
//            Assert.Equal("Value cannot be null.\r\nParameter name: requestBaseAddress", exceptionResult.Message);
//        }

//        [Fact]
//        public void JobProfileServiceRefreshSegmentsReturnsNullWhenProfileNotReplaced()
//        {
//            // arrange
//            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
//            var jobProfileModel = A.Fake<JobProfileModel>();
//            var expectedResult = A.Dummy<JobProfileModel>();

//            A.CallTo(() => segmentService.LoadAsync());
//            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.BadRequest);

//            // act
//            var result = jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, jobProfileModel, dummyBaseAddressUri).Result;

//            // assert
//            A.CallTo(() => segmentService.LoadAsync()).MustHaveHappenedOnceExactly();
//            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
//            A.Equals(result, expectedResult);
//        }

//        [Fact]
//        public void JobProfileServiceRefreshSegmentsReturnsNullWhenMissingRepository()
//        {
//            // arrange
//            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
//            var jobProfileModel = A.Fake<JobProfileModel>();
//            Data.Models.JobProfileModel expectedResult = null;

//            A.CallTo(() => segmentService.LoadAsync());
//            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).Returns(HttpStatusCode.FailedDependency);

//            // act
//            var result = jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, jobProfileModel, dummyBaseAddressUri).Result;

//            // assert
//            A.CallTo(() => segmentService.LoadAsync()).MustHaveHappenedOnceExactly();
//            A.CallTo(() => repository.UpsertAsync(jobProfileModel)).MustHaveHappenedOnceExactly();
//            A.Equals(result, expectedResult);
//        }
//    }
//}
