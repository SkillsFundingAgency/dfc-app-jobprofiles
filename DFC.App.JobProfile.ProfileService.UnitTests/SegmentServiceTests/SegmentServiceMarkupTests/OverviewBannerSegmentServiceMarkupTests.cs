using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceMarkupTests
{
    [Trait("Profile Service", "Overview Banner Segment Service Markup Tests")]
    public class OverviewBannerSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly OverviewBannerSegmentModel ExpectedResult = new OverviewBannerSegmentModel
        {
            Updated = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<OverviewBannerSegmentService> logger;
        private readonly OverviewBannerSegmentClientOptions overviewBannerSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(OverviewBannerSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public OverviewBannerSegmentServiceMarkupTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await overviewBannerSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.Updated);
                }
            }
        }

        [Fact]
        public async Task OverviewBannerSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            OverviewBannerSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var overviewBannerSegmentService = new OverviewBannerSegmentService(httpClient, logger, overviewBannerSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await overviewBannerSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
