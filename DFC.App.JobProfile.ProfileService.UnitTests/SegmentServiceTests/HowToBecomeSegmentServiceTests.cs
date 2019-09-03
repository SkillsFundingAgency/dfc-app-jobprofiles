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
    [Trait("Profile Service", "How To Become Segment Service Tests")]
    public class HowToBecomeSegmentServiceTests
    {
        private const string ExpectedLastReviewed = "2019-08-30T08:00:00";
        private static readonly HowToBecomeSegmentModel ExpectedResult = new HowToBecomeSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<HowToBecomeSegmentService> logger;
        private readonly HowToBecomeSegmentClientOptions howToBecomeSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpectedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public HowToBecomeSegmentServiceTests()
        {
            logger = A.Fake<ILogger<HowToBecomeSegmentService>>();
            howToBecomeSegmentClientOptions = A.Fake<HowToBecomeSegmentClientOptions>();

            howToBecomeSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            howToBecomeSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task HowToBecomeSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions);

                    // act
                    var results = await howToBecomeSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task HowToBecomeSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            HowToBecomeSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions);

                    // act
                    var results = await howToBecomeSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
