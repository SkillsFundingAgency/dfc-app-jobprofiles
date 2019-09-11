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
    [Trait("Profile Service", "Career Path Segment Service Health Tests")]
    public class CareerPathSegmentServiceHealthTests
    {
        private readonly ILogger<CareerPathSegmentService> logger;
        private readonly CareerPathSegmentClientOptions careerPathSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.CareerPath\",\"message\":\"Document store is available\"}]}";

        public CareerPathSegmentServiceHealthTests()
        {
            logger = A.Fake<ILogger<CareerPathSegmentService>>();
            careerPathSegmentClientOptions = A.Fake<CareerPathSegmentClientOptions>();

            careerPathSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            careerPathSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions);

                    // act
                    var results = await careerPathSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.Count, 1);
                }
            }
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions);

                    // act
                    var results = await careerPathSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}