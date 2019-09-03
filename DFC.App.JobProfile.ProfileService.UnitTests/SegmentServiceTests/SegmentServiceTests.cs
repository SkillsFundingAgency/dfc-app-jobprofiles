using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests
{
    [Trait("Profile Service", "Segment Service Tests")]
    public class SegmentServiceTests
    {
        private readonly ILogger<SegmentService> logger;
        private readonly ICareerPathSegmentService careerPathSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IHowToBecomeSegmentService howToBecomeSegmentService;
        private readonly IOverviewBannerSegmentService overviewBannerSegmentService;
        private readonly IRelatedCareersSegmentService relatedCareersSegmentService;
        private readonly IWhatItTakesSegmentService whatItTakesSegmentService;

        private readonly CareerPathSegmentModel expectedResultForCareerPath = new CareerPathSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-1),
            Content = "<p>CareerPath data</p>",
        };

        private readonly CurrentOpportunitiesSegmentModel expectedResultForCurrentOpportunities = new CurrentOpportunitiesSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-2),
            Content = "<p> CurrentOpportunities data</p>",
        };

        private readonly HowToBecomeSegmentModel expectedResultForHowToBecome = new HowToBecomeSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-3),
            Content = "<p>HowToBecome data</p>",
        };

        private readonly OverviewBannerSegmentModel expectedResultForOverviewBanner = new OverviewBannerSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-4),
            Content = "<p>OverviewBanner data</p>",
        };

        private readonly RelatedCareersSegmentModel expectedResultForRelatedCareers = new RelatedCareersSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-5),
            Content = "<p>RelatedCareers data</p>",
        };

        private readonly WhatItTakesSegmentModel expectedResultForWhatItTakes = new WhatItTakesSegmentModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-6),
            Content = "<p>WhatItTakes data</p>",
        };
        
        public SegmentServiceTests()
        {
            logger = A.Fake<ILogger<SegmentService>>();
            careerPathSegmentService = A.Fake<CareerPathSegmentService>();
            currentOpportunitiesSegmentService = A.Fake<CurrentOpportunitiesSegmentService>();
            howToBecomeSegmentService = A.Fake<HowToBecomeSegmentService>();
            overviewBannerSegmentService = A.Fake<OverviewBannerSegmentService>();
            relatedCareersSegmentService = A.Fake<RelatedCareersSegmentService>();
            whatItTakesSegmentService = A.Fake<WhatItTakesSegmentService>();

            var baseAddress = new Uri("https://nowhere.com");
            const string endpoint = "segment/{0}/contents";

            careerPathSegmentService.SegmentClientOptions = new CareerPathSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{careerPathSegmentService.GetType().Name} is offline</h3>",
            };
            currentOpportunitiesSegmentService.SegmentClientOptions = new CurrentOpportunitiesSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{currentOpportunitiesSegmentService.GetType().Name} is offline</h3>",
            };
            howToBecomeSegmentService.SegmentClientOptions = new HowToBecomeSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{howToBecomeSegmentService.GetType().Name} is offline</h3>",
            };
            overviewBannerSegmentService.SegmentClientOptions = new OverviewBannerSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{overviewBannerSegmentService.GetType().Name} is offline</h3>",
            };
            relatedCareersSegmentService.SegmentClientOptions = new RelatedCareersSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{relatedCareersSegmentService.GetType().Name} is offline</h3>",
            };
            whatItTakesSegmentService.SegmentClientOptions = new WhatItTakesSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{whatItTakesSegmentService.GetType().Name} is offline</h3>",
            };
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenNoRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshAllSegments = false,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenAllRefreshRequired()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshAllSegments = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForCareerPath));
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForCurrentOpportunities));
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForHowToBecome));
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForOverviewBanner));
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForRelatedCareers));
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForWhatItTakes));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().Be(expectedResultForCareerPath.LastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(expectedResultForCareerPath.Content);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().Be(expectedResultForCurrentOpportunities.LastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(expectedResultForCurrentOpportunities.Content);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().Be(expectedResultForHowToBecome.LastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(expectedResultForHowToBecome.Content);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().Be(expectedResultForOverviewBanner.LastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(expectedResultForOverviewBanner.Content);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().Be(expectedResultForRelatedCareers.LastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(expectedResultForRelatedCareers.Content);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().Be(expectedResultForWhatItTakes.LastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(expectedResultForWhatItTakes.Content);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenCareerOathOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshCareerPathSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForCareerPath));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().Be(expectedResultForCareerPath.LastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(expectedResultForCareerPath.Content);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenCurrentOpportunitiesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshCurrentOpportunitiesSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForCurrentOpportunities));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().Be(expectedResultForCurrentOpportunities.LastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(expectedResultForCurrentOpportunities.Content);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenHowToBecomeOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshHowToBecomeSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForHowToBecome));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().Be(expectedResultForHowToBecome.LastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(expectedResultForHowToBecome.Content);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenOverviewBannerOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshOverviewBannerSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForOverviewBanner));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().Be(expectedResultForOverviewBanner.LastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(expectedResultForOverviewBanner.Content);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenRelatedCareersOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshRelatedCareersSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForRelatedCareers));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().Be(expectedResultForRelatedCareers.LastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(expectedResultForRelatedCareers.Content);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceReturnsSuccessWhenWhatItTakesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedLastReviewed = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshWhatItTakesSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Segments = new SegmentsModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).Returns(Task.FromResult(expectedResultForWhatItTakes));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadAsync(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            jobProfileModel.Segments.CareerPath.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CareerPath.Content.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.CurrentOpportunities.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Content.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.HowToBecome.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.HowToBecome.Content.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.OverviewBanner.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.OverviewBanner.Content.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.RelatedCareers.LastReviewed.Should().BeAfter(expectedLastReviewed);
            jobProfileModel.Segments.RelatedCareers.Content.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Segments.WhatItTakes.LastReviewed.Should().Be(expectedResultForWhatItTakes.LastReviewed);
            jobProfileModel.Segments.WhatItTakes.Content.Should().Be(expectedResultForWhatItTakes.Content);
        }
    }
}
