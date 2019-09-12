using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests
{
    [Trait("Profile Service", "Segment Service Load Tests")]
    public class SegmentServiceLoadAsyncTests
    {
        private readonly ILogger<SegmentService> logger;
        private readonly ICareerPathSegmentService careerPathSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IHowToBecomeSegmentService howToBecomeSegmentService;
        private readonly IOverviewBannerSegmentService overviewBannerSegmentService;
        private readonly IRelatedCareersSegmentService relatedCareersSegmentService;
        private readonly IWhatItTakesSegmentService whatItTakesSegmentService;
        private readonly IWhatYouWillDoSegmentService whatYouWillDoSegmentService;

        private readonly CareerPathSegmentModel expectedResultForCareerPath = new CareerPathSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-1),
        };

        private readonly CurrentOpportunitiesSegmentModel expectedResultForCurrentOpportunities = new CurrentOpportunitiesSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-2),
        };

        private readonly HowToBecomeSegmentModel expectedResultForHowToBecome = new HowToBecomeSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-3),
        };

        private readonly OverviewBannerSegmentModel expectedResultForOverviewBanner = new OverviewBannerSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-4),
        };

        private readonly RelatedCareersSegmentModel expectedResultForRelatedCareers = new RelatedCareersSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-5),
        };

        private readonly WhatItTakesSegmentModel expectedResultForWhatItTakes = new WhatItTakesSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-6),
        };

        private readonly WhatYouWillDoSegmentModel expectedResultForWhatYouWillDo = new WhatYouWillDoSegmentModel
        {
            Updated = DateTime.UtcNow.AddDays(-6),
        };

        private readonly SegmentsMarkupModel expectedResultForMarkup = new SegmentsMarkupModel
        {
            CareerPath = "<p>CareerPath data</p>",
            CurrentOpportunities = "<p> CurrentOpportunities data</p>",
            HowToBecome = "<p>HowToBecome data</p>",
            OverviewBanner = "<p>OverviewBanner data</p>",
            RelatedCareers = "<p>RelatedCareers data</p>",
            WhatItTakes = "<p>WhatItTakes data</p>",
            WhatYouWillDo = "<p>WhatYouWillDo data</p>",
        };

        public SegmentServiceLoadAsyncTests()
        {
            logger = A.Fake<ILogger<SegmentService>>();
            careerPathSegmentService = A.Fake<CareerPathSegmentService>();
            currentOpportunitiesSegmentService = A.Fake<CurrentOpportunitiesSegmentService>();
            howToBecomeSegmentService = A.Fake<HowToBecomeSegmentService>();
            overviewBannerSegmentService = A.Fake<OverviewBannerSegmentService>();
            relatedCareersSegmentService = A.Fake<RelatedCareersSegmentService>();
            whatItTakesSegmentService = A.Fake<WhatItTakesSegmentService>();
            whatYouWillDoSegmentService = A.Fake<WhatYouWillDoSegmentService>();

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
            whatYouWillDoSegmentService.SegmentClientOptions = new WhatYouWillDoSegmentClientOptions
            {
                BaseAddress = baseAddress,
                Endpoint = endpoint,
                OfflineHtml = $"<h3>{whatYouWillDoSegmentService.GetType().Name} is offline</h3>",
            };
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenNoRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenAllRefreshRequired()
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForCareerPath));
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForCurrentOpportunities));
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForHowToBecome));
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForOverviewBanner));
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForRelatedCareers));
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForWhatItTakes));
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForWhatYouWillDo));

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.CareerPath));
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.CurrentOpportunities));
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.HowToBecome));
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.OverviewBanner));
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.RelatedCareers));
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.WhatItTakes));
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.WhatYouWillDo));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();

            jobProfileModel.Data.CareerPath.Updated.Should().Be(expectedResultForCareerPath.Updated);
            jobProfileModel.Markup.CareerPath.Should().Be(expectedResultForMarkup.CareerPath);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().Be(expectedResultForCurrentOpportunities.Updated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(expectedResultForMarkup.CurrentOpportunities);
            jobProfileModel.Data.HowToBecome.Updated.Should().Be(expectedResultForHowToBecome.Updated);
            jobProfileModel.Markup.HowToBecome.Should().Be(expectedResultForMarkup.HowToBecome);
            jobProfileModel.Data.OverviewBanner.Updated.Should().Be(expectedResultForOverviewBanner.Updated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(expectedResultForMarkup.OverviewBanner);
            jobProfileModel.Data.RelatedCareers.Updated.Should().Be(expectedResultForRelatedCareers.Updated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(expectedResultForMarkup.RelatedCareers);
            jobProfileModel.Data.WhatItTakes.Updated.Should().Be(expectedResultForWhatItTakes.Updated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(expectedResultForMarkup.WhatItTakes);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().Be(expectedResultForWhatYouWillDo.Updated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(expectedResultForMarkup.WhatYouWillDo);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenCareerOathOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForCareerPath));
            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.CareerPath));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().Be(expectedResultForCareerPath.Updated);
            jobProfileModel.Markup.CareerPath.Should().Be(expectedResultForMarkup.CareerPath);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenCurrentOpportunitiesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForCurrentOpportunities));
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.CurrentOpportunities));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().Be(expectedResultForCurrentOpportunities.Updated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(expectedResultForMarkup.CurrentOpportunities);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenHowToBecomeOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForHowToBecome));
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.HowToBecome));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().Be(expectedResultForHowToBecome.Updated);
            jobProfileModel.Markup.HowToBecome.Should().Be(expectedResultForMarkup.HowToBecome);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenOverviewBannerOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForOverviewBanner));
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.OverviewBanner));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().Be(expectedResultForOverviewBanner.Updated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(expectedResultForMarkup.OverviewBanner);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenRelatedCareersOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForRelatedCareers));
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.RelatedCareers));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().Be(expectedResultForRelatedCareers.Updated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(expectedResultForMarkup.RelatedCareers);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenWhatItTakesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
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
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForWhatItTakes));
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.WhatItTakes));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustNotHaveHappened();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustNotHaveHappened();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().Be(expectedResultForWhatItTakes.Updated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(expectedResultForMarkup.WhatItTakes);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenWhatYouWillDoOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                RefreshWhatYouWillDoSegment = true,
            };
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createOrUpdateJobProfileModel.DocumentId,
                CanonicalName = createOrUpdateJobProfileModel.CanonicalName,
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };
            var segmentService = new SegmentService(
                                                        logger,
                                                        careerPathSegmentService,
                                                        currentOpportunitiesSegmentService,
                                                        howToBecomeSegmentService,
                                                        overviewBannerSegmentService,
                                                        relatedCareersSegmentService,
                                                        whatItTakesSegmentService,
                                                        whatYouWillDoSegmentService)
            {
                CreateOrUpdateJobProfileModel = createOrUpdateJobProfileModel,
                JobProfileModel = jobProfileModel,
            };

            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).Returns(Task.FromResult(expectedResultForWhatYouWillDo));
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).Returns(Task.FromResult(expectedResultForMarkup.WhatYouWillDo));

            // act
            await segmentService.LoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadDataAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadDataAsync()).MustHaveHappenedOnceExactly();

            A.CallTo(() => careerPathSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => currentOpportunitiesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => howToBecomeSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => overviewBannerSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => relatedCareersSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatItTakesSegmentService.LoadMarkupAsync()).MustNotHaveHappened();
            A.CallTo(() => whatYouWillDoSegmentService.LoadMarkupAsync()).MustHaveHappenedOnceExactly();

            jobProfileModel.Data.CareerPath.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CareerPath.Should().Be(careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.CurrentOpportunities.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.CurrentOpportunities.Should().Be(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.HowToBecome.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.HowToBecome.Should().Be(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.OverviewBanner.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.OverviewBanner.Should().Be(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.RelatedCareers.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.RelatedCareers.Should().Be(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatItTakes.Updated.Should().BeAfter(expectedUpdated);
            jobProfileModel.Markup.WhatItTakes.Should().Be(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            jobProfileModel.Data.WhatYouWillDo.Updated.Should().Be(expectedResultForWhatYouWillDo.Updated);
            jobProfileModel.Markup.WhatYouWillDo.Should().Be(expectedResultForMarkup.WhatYouWillDo);
        }
    }
}
