using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.ProfileService.SegmentServices;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceHealthTests
{
    [Trait("Profile Service", "Current Opportunities Segment Service Health Tests")]
    public class CurrentOpportunitiesSegmentServiceHealthTests
    {
        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly CurrentOpportunitiesSegmentClientOptions currentOpportunitiesSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.CurrentOpportunities\",\"message\":\"Document store is available\"}]}";

        public CurrentOpportunitiesSegmentServiceHealthTests()
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
                    var results = await currentOpportunitiesSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.HealthItems.Count, 1);
                }
            }
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions);

                    // act
                    var results = await currentOpportunitiesSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}