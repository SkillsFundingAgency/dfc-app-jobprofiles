using DFC.App.JobProfile.Data.HttpClientPolicies;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceHealthTests
{
    [Trait("Profile Service", "Overview Banner Segment Service Health Tests")]
    public class OverviewBannerSegmentServiceHealthTests
    {
        private readonly ILogger<OverviewBannerSegmentService> logger;
        private readonly OverviewBannerSegmentClientOptions overviewBannerSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.OverviewBanner\",\"message\":\"Document store is available\"}]}";

        public OverviewBannerSegmentServiceHealthTests()
        {
            logger = A.Fake<ILogger<OverviewBannerSegmentService>>();
            overviewBannerSegmentClientOptions = A.Fake<OverviewBannerSegmentClientOptions>();

            overviewBannerSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            overviewBannerSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task OverviewBannerSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions);

                    // act
                    var results = await overviewBannerSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.HealthItems.Count, 1);
                }
            }
        }

        [Fact]
        public async Task OverviewBannerSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions);

                    // act
                    var results = await overviewBannerSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}