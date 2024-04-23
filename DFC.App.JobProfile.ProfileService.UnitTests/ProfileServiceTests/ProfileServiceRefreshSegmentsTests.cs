using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
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
        private readonly ICosmosRepository<JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine fakeRazorTemplateEngine;
        private readonly IConfiguration fakeConfiguration;

        public ProfileServiceRefreshSegmentsTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();

            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            fakeConfiguration = A.Fake<IConfiguration>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration);
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
            refreshJobProfileSegmentModel.Segment = Data.JobProfileSegment.Overview;
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            existingJobProfileModel.Segments = new List<SegmentModel>
            {
                new SegmentModel
                {
                    Segment = Data.JobProfileSegment.Overview,
                },
            };
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

        [Fact]
        public async Task JobProfileServiceRefreshSegmentReturnsOfflineMarkupWhenFailedAndExistingSegmentNull()
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
            segmentModel.RefreshStatus = Data.Enums.RefreshStatus.Failed;
            var offlineModel = new OfflineSegmentModel
            {
                OfflineMarkup = new HtmlString("This is offline markup"),
                OfflineJson = "This is offline json",
            };

            var expectedResult = HttpStatusCode.FailedDependency;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).Returns(segmentModel);
            A.CallTo(() => segmentService.GetOfflineSegment(refreshJobProfileSegmentModel.Segment)).Returns(offlineModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // act
            var result = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.That.Matches(m =>
                m.Segments[0].Markup.Value == offlineModel.OfflineMarkup.Value
                && m.Segments[0].Json == offlineModel.OfflineJson))).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceRefreshSegmentReturnsExistingMarkupWhenFailed()
        {
            // arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            var existingModel = new SegmentModel
            {
                Segment = Data.JobProfileSegment.Overview,
                Markup = new HtmlString("This is existing markup"),
                Json = "This is existing json",
            };

            existingJobProfileModel.Segments = new List<SegmentModel>
            {
                existingModel,
            };

            var jobProfileModel = A.Fake<JobProfileModel>();
            var existingSegmentModel = A.Dummy<SegmentModel>();
            var segmentModel = A.Dummy<SegmentModel>();
            segmentModel.RefreshStatus = Data.Enums.RefreshStatus.Failed;
            var offlineModel = new OfflineSegmentModel
            {
                OfflineMarkup = new HtmlString("This is offline markup"),
                OfflineJson = "This is offline json",
            };

            var expectedResult = HttpStatusCode.FailedDependency;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).Returns(segmentModel);
            A.CallTo(() => segmentService.GetOfflineSegment(refreshJobProfileSegmentModel.Segment)).Returns(offlineModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // act
            var result = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.That.Matches(m =>
                m.Segments[0].Markup.Value == existingModel.Markup.Value
                && m.Segments[0].Json == existingModel.Json))).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceRefreshSegmentReturnsNewMarkupWhenSuccess()
        {
            // arrange
            var refreshJobProfileSegmentModel = A.Fake<RefreshJobProfileSegment>();
            var existingJobProfileModel = A.Fake<JobProfileModel>();
            var existingModel = new SegmentModel
            {
                Segment = Data.JobProfileSegment.Overview,
                Markup = new HtmlString("This is existing markup"),
                Json = "This is existing json",
            };

            existingJobProfileModel.Segments = new List<SegmentModel>
            {
                existingModel,
            };

            var jobProfileModel = A.Fake<JobProfileModel>();
            var existingSegmentModel = A.Dummy<SegmentModel>();
            var segmentModel = A.Dummy<SegmentModel>();
            segmentModel.Json = "This is new Json";
            segmentModel.Markup = new HtmlString("This is new markup");

            var offlineModel = new OfflineSegmentModel
            {
                OfflineMarkup = new HtmlString("This is offline markup"),
                OfflineJson = "This is offline json",
            };

            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(existingJobProfileModel);
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).Returns(segmentModel);
            A.CallTo(() => segmentService.GetOfflineSegment(refreshJobProfileSegmentModel.Segment)).Returns(offlineModel);
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.Ignored)).Returns(HttpStatusCode.OK);

            // act
            var result = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.UpsertAsync(A<JobProfileModel>.That.Matches(m =>
                m.Segments[0].Markup.Value == segmentModel.Markup.Value
                && m.Segments[0].Json == segmentModel.Json))).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResult);
        }
    }
}