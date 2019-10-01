using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerDataModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceDataTests
{
    [Trait("Profile Service", "Overview Banner Segment Service Data Tests")]
    public class OverviewBannerSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly OverviewBannerSegmentModel ExpectedResult = new OverviewBannerSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<OverviewBannerSegmentService> logger;
        private readonly OverviewBannerSegmentClientOptions overviewBannerSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public OverviewBannerSegmentServiceDataTests()
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
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await overviewBannerSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task OverviewBannerSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            OverviewBannerSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await overviewBannerSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
