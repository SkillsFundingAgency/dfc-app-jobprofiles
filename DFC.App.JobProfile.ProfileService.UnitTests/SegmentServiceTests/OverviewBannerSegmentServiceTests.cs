using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests
{
    [Trait("Profile Service", "Overview Banner Segment Service Tests")]
    public class OverviewBannerSegmentServiceTests
    {
        private const string ExpecedLastReviewed = "2019-08-30T08:00:00";
        private static readonly OverviewBannerSegmentModel ExpectedResult = new OverviewBannerSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpecedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<OverviewBannerSegmentService> logger;
        private readonly OverviewBannerSegmentClientOptions overviewBannerSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpecedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public OverviewBannerSegmentServiceTests()
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
                    var results = await overviewBannerSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task OverviewBannerSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            OverviewBannerSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions);

                    // act
                    var results = await overviewBannerSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
