using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests
{
    [Trait("Profile Service", "Segment Health Check Tests")]
    public class SegmentServiceSegmentsHealthCheckAsyncTests
    {
        private const int NumberOfSegmentServices = 7;

        private readonly ILogger<SegmentService> logger;
        private readonly ISegmentRefreshService<CareerPathSegmentClientOptions> careerPathSegmentService;
        private readonly ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions> currentOpportunitiesSegmentService;
        private readonly ISegmentRefreshService<HowToBecomeSegmentClientOptions> howToBecomeSegmentService;
        private readonly ISegmentRefreshService<OverviewBannerSegmentClientOptions> overviewBannerSegmentService;
        private readonly ISegmentRefreshService<RelatedCareersSegmentClientOptions> relatedCareersSegmentService;
        private readonly ISegmentRefreshService<WhatItTakesSegmentClientOptions> whatItTakesSegmentService;
        private readonly ISegmentRefreshService<WhatYouWillDoSegmentClientOptions> whatYouWillDoSegmentService;

        public SegmentServiceSegmentsHealthCheckAsyncTests()
        {
            logger = A.Fake<ILogger<SegmentService>>();
            careerPathSegmentService = A.Fake<ISegmentRefreshService<CareerPathSegmentClientOptions>>();
            currentOpportunitiesSegmentService = A.Fake<ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions>>();
            howToBecomeSegmentService = A.Fake<ISegmentRefreshService<HowToBecomeSegmentClientOptions>>();
            overviewBannerSegmentService = A.Fake<ISegmentRefreshService<OverviewBannerSegmentClientOptions>>();
            relatedCareersSegmentService = A.Fake<ISegmentRefreshService<RelatedCareersSegmentClientOptions>>();
            whatItTakesSegmentService = A.Fake<ISegmentRefreshService<WhatItTakesSegmentClientOptions>>();
            whatYouWillDoSegmentService = A.Fake<ISegmentRefreshService<WhatYouWillDoSegmentClientOptions>>();

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
        public async Task SegmentServiceSegmentsHealthCheckAsyncReturnsSuccess()
        {
            // arrange
            var segmentService = new SegmentService(
                logger,
                overviewBannerSegmentService,
                howToBecomeSegmentService,
                whatItTakesSegmentService,
                whatYouWillDoSegmentService,
                careerPathSegmentService,
                currentOpportunitiesSegmentService,
                relatedCareersSegmentService);

            var expectedResult = new HealthCheckItems
            {
                Source = new Uri("http://somewhere.com"),
                HealthItems = new List<HealthCheckItem>
                {
                    new HealthCheckItem
                    {
                        Service = "Unit test",
                        Message = "All ok",
                    },
                },
            };

            A.CallTo(() => careerPathSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => currentOpportunitiesSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => howToBecomeSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => overviewBannerSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => relatedCareersSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => whatItTakesSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));
            A.CallTo(() => whatYouWillDoSegmentService.HealthCheckAsync()).Returns(Task.FromResult(expectedResult));

            // act
            var results = await segmentService.SegmentsHealthCheckAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => careerPathSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => currentOpportunitiesSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => howToBecomeSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => overviewBannerSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => relatedCareersSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatItTakesSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => whatYouWillDoSegmentService.HealthCheckAsync()).MustHaveHappenedOnceExactly();

            results.Count.Should().Be(NumberOfSegmentServices);
            results[0].Service.Should().Be(expectedResult.HealthItems[0].Service);
            results[0].Message.Should().Be(expectedResult.HealthItems[0].Message);
        }
    }
}