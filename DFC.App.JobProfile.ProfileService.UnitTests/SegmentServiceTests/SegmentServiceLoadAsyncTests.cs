using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
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
        private readonly Uri dummyBaseAddressUri = new Uri("https://localhost:12345/");
        private readonly ILogger<SegmentService> logger;
        private readonly ICareerPathSegmentService careerPathSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IHowToBecomeSegmentService howToBecomeSegmentService;
        private readonly IOverviewBannerSegmentService overviewBannerSegmentService;
        private readonly IRelatedCareersSegmentService relatedCareersSegmentService;
        private readonly IWhatItTakesSegmentService whatItTakesSegmentService;
        private readonly IWhatYouWillDoSegmentService whatYouWillDoSegmentService;

        private readonly CareerPathSegmentDataModel expectedResultForCareerPath = new CareerPathSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-1),
        };

        private readonly CurrentOpportunitiesSegmentDataModel expectedResultForCurrentOpportunities = new CurrentOpportunitiesSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-2),
        };

        private readonly HowToBecomeSegmentDataModel expectedResultForHowToBecome = new HowToBecomeSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-3),
        };

        private readonly OverviewBannerSegmentDataModel expectedResultForOverviewBanner = new OverviewBannerSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-4),
        };

        private readonly RelatedCareersSegmentDataModel expectedResultForRelatedCareers = new RelatedCareersSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-5),
        };

        private readonly JobProfileSkillSegmentDataModel expectedResultForWhatItTakes = new JobProfileSkillSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-6),
        };

        private readonly JobProfileTasksSegmentDataModel expectedResultForWhatYouWillDo = new JobProfileTasksSegmentDataModel
        {
            LastReviewed = DateTime.UtcNow.AddDays(-7),
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
            careerPathSegmentService = A.Fake<ICareerPathSegmentService>();
            currentOpportunitiesSegmentService = A.Fake<ICurrentOpportunitiesSegmentService>();
            howToBecomeSegmentService = A.Fake<IHowToBecomeSegmentService>();
            overviewBannerSegmentService = A.Fake<IOverviewBannerSegmentService>();
            relatedCareersSegmentService = A.Fake<IRelatedCareersSegmentService>();
            whatItTakesSegmentService = A.Fake<IWhatItTakesSegmentService>();
            whatYouWillDoSegmentService = A.Fake<IWhatYouWillDoSegmentService>();

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
            var toRefresh = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = Data.JobProfileSegment.Overview,
            };
            
            var jobProfileModel = new JobProfileModel
            {
                DocumentId = toRefresh.JobProfileId,
                CanonicalName = toRefresh.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new List<SegmentModel>(),
            };

            var segmentService = new SegmentService(
                logger,
                careerPathSegmentService,
                currentOpportunitiesSegmentService,
                howToBecomeSegmentService,
                overviewBannerSegmentService,
                relatedCareersSegmentService,
                whatItTakesSegmentService,
                whatYouWillDoSegmentService);

            // act
            await segmentService.RefreshSegmentAsync(toRefresh).ConfigureAwait(false);

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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenAllRefreshRequired()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = null,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.LastReviewed.Should().Be(expectedResultForCareerPath.LastReviewed);
            jobProfileModel.Segments.CareerPath.Should().Be(expectedResultForMarkup.CareerPath);
            jobProfileModel.Data.CurrentOpportunities.LastReviewed.Should().Be(expectedResultForCurrentOpportunities.LastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Should().Be(expectedResultForMarkup.CurrentOpportunities);
            jobProfileModel.Data.HowToBecome.LastReviewed.Should().Be(expectedResultForHowToBecome.LastReviewed);
            jobProfileModel.Segments.HowToBecome.Should().Be(expectedResultForMarkup.HowToBecome);
            jobProfileModel.Data.OverviewBanner.LastReviewed.Should().Be(expectedResultForOverviewBanner.LastReviewed);
            jobProfileModel.Segments.OverviewBanner.Should().Be(expectedResultForMarkup.OverviewBanner);
            jobProfileModel.Data.RelatedCareers.LastReviewed.Should().Be(expectedResultForRelatedCareers.LastReviewed);
            jobProfileModel.Segments.RelatedCareers.Should().Be(expectedResultForMarkup.RelatedCareers);
            jobProfileModel.Data.WhatItTakes.LastReviewed.Should().Be(expectedResultForWhatItTakes.LastReviewed);
            jobProfileModel.Segments.WhatItTakes.Should().Be(expectedResultForMarkup.WhatItTakes);
            jobProfileModel.Data.WhatYouWillDo.LastReviewed.Should().Be(expectedResultForWhatYouWillDo.LastReviewed);
            jobProfileModel.Segments.WhatYouWillDo.Should().Be(expectedResultForMarkup.WhatYouWillDo);
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenCareerPathOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = CareerPathSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.LastReviewed.Should().Be(expectedResultForCareerPath.LastReviewed);
            jobProfileModel.Segments.CareerPath.Should().Be(expectedResultForMarkup.CareerPath);
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenCurrentOpportunitiesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = CurrentOpportunitiesSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.LastReviewed.Should().Be(expectedResultForCurrentOpportunities.LastReviewed);
            jobProfileModel.Segments.CurrentOpportunities.Should().Be(expectedResultForMarkup.CurrentOpportunities);
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenHowToBecomeOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = HowToBecomeSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.LastReviewed.Should().Be(expectedResultForHowToBecome.LastReviewed);
            jobProfileModel.Segments.HowToBecome.Should().Be(expectedResultForMarkup.HowToBecome);
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenOverviewBannerOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = OverviewBannerSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.LastReviewed.Should().Be(expectedResultForOverviewBanner.LastReviewed);
            jobProfileModel.Segments.OverviewBanner.Should().Be(expectedResultForMarkup.OverviewBanner);
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenRelatedCareersOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = RelatedCareersSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.LastReviewed.Should().Be(expectedResultForRelatedCareers.LastReviewed);
            jobProfileModel.Segments.RelatedCareers.Should().Be(expectedResultForMarkup.RelatedCareers);
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenWhatItTakesOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = JobProfileSkillSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.LastReviewed.Should().Be(expectedResultForWhatItTakes.LastReviewed);
            jobProfileModel.Segments.WhatItTakes.Should().Be(expectedResultForMarkup.WhatItTakes);
            jobProfileModel.Data.WhatYouWillDo.Should().BeNull();
            jobProfileModel.Segments.WhatYouWillDo.Should().BeNull();
        }

        [Fact]
        public async Task SegmentServiceLoadAsyncReturnsSuccessWhenWhatYouWillDoOnlyRefreshRequired()
        {
            // arrange
            DateTime expectedUpdated = DateTime.UtcNow;
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                Segment = JobProfileTasksSegmentDataModel.SegmentName,
            };
            var jobProfileModel = new Data.Models.JobProfileModel
            {
                DocumentId = refreshJobProfileSegmentModel.JobProfileId,
                CanonicalName = refreshJobProfileSegmentModel.CanonicalName,
                MetaTags = new MetaTags(),
                Segments = new SegmentsMarkupModel(),
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
                RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel,
                JobProfileModel = jobProfileModel,
                RequestBaseAddress = dummyBaseAddressUri,
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

            jobProfileModel.Data.CareerPath.Should().BeNull();
            jobProfileModel.Segments.CareerPath.Should().BeNull();
            jobProfileModel.Data.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Segments.CurrentOpportunities.Should().BeNull();
            jobProfileModel.Data.HowToBecome.Should().BeNull();
            jobProfileModel.Segments.HowToBecome.Should().BeNull();
            jobProfileModel.Data.OverviewBanner.Should().BeNull();
            jobProfileModel.Segments.OverviewBanner.Should().BeNull();
            jobProfileModel.Data.RelatedCareers.Should().BeNull();
            jobProfileModel.Segments.RelatedCareers.Should().BeNull();
            jobProfileModel.Data.WhatItTakes.Should().BeNull();
            jobProfileModel.Segments.WhatItTakes.Should().BeNull();
            jobProfileModel.Data.WhatYouWillDo.LastReviewed.Should().Be(expectedResultForWhatYouWillDo.LastReviewed);
            jobProfileModel.Segments.WhatYouWillDo.Should().Be(expectedResultForMarkup.WhatYouWillDo);
        }
    }
}