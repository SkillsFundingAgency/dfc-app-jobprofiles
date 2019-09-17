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
    [Trait("Profile Service", "Related Careers Segment Service Health Tests")]
    public class RelatedCareersSegmentServiceHealthTests
    {
        private readonly ILogger<RelatedCareersSegmentService> logger;
        private readonly RelatedCareersSegmentClientOptions relatedCareersSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.RelatedCareers\",\"message\":\"Document store is available\"}]}";

        public RelatedCareersSegmentServiceHealthTests()
        {
            logger = A.Fake<ILogger<RelatedCareersSegmentService>>();
            relatedCareersSegmentClientOptions = A.Fake<RelatedCareersSegmentClientOptions>();

            relatedCareersSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            relatedCareersSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task RelatedCareersSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions);

                    // act
                    var results = await relatedCareersSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.HealthItems.Count, 1);
                }
            }
        }

        [Fact]
        public async Task RelatedCareersSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions);

                    // act
                    var results = await relatedCareersSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}