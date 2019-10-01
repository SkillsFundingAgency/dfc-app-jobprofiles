using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesDataModels;
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
    [Trait("Profile Service", "Current Opportunities Segment Service Markup Tests")]
    public class CurrentOpportunitiesSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly CurrentOpportunitiesSegmentModel ExpectedResult = new CurrentOpportunitiesSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly CurrentOpportunitiesSegmentClientOptions currentOpportunitiesSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(CurrentOpportunitiesSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public CurrentOpportunitiesSegmentServiceMarkupTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            CurrentOpportunitiesSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
