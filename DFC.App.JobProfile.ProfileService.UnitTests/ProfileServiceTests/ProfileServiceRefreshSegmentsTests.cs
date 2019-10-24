using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "RefreshSegments Tests")]
    public class ProfileServiceRefreshSegmentsTests
    {
        private readonly Uri dummyBaseAddressUri = new Uri("https://localhost:12345/");
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceRefreshSegmentsTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();

            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper);
        }

        [Fact]
        public async Task JobProfileServiceRefreshSegmentsReturnsSuccessWhenProfileReplacedAsync()
        {
            // arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            existingJobProfileModel.Segments = new List<SegmentModel>
            {
                new SegmentModel
                {
                     Segment = Data.JobProfileSegment.Overview,
                },
            };

            var jobProfileModel = A.Fake<JobProfileModel>();
            var existingSegmentModel = A.Dummy<SegmentModel>();
            var segmentModel = A.Dummy<SegmentModel>();

            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).Returns(segmentModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // act
            var result = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceRefreshSegmentsReturnsArgumentNullExceptionWhenNullParam1IsUsed()
        {
            // arrange
            var jobProfileModel = A.Fake<JobProfileModel>();

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.RefreshSegmentsAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            exceptionResult.Should().BeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public async Task JobProfileServiceRefreshSegmentsReturnsBadRequestWhenProfileNotReplacedAsync()
        {
            // arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            existingJobProfileModel.Segments = A.Fake<IList<SegmentModel>>();
            var jobProfileModel = A.Fake<JobProfileModel>();
            var segmentModel = A.Dummy<SegmentModel>();

            var expectedResult = HttpStatusCode.BadRequest;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).Returns(segmentModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().Be(expectedResult);
        }
    }
}