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
    [Trait("Profile Service", "Current Opportunities Segment Service Tests")]
    public class CurrentOpportunitiesSegmentServiceTests
    {
        private const string ExpectedLastReviewed = "2019-08-30T08:00:00";
        private static readonly CurrentOpportunitiesSegmentModel ExpectedResult = new CurrentOpportunitiesSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly CurrentOpportunitiesSegmentClientOptions currentOpportunitiesSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpectedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public CurrentOpportunitiesSegmentServiceTests()
        {
            logger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            currentOpportunitiesSegmentClientOptions = A.Fake<CurrentOpportunitiesSegmentClientOptions>();

            currentOpportunitiesSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            currentOpportunitiesSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions);

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            CurrentOpportunitiesSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions);

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
