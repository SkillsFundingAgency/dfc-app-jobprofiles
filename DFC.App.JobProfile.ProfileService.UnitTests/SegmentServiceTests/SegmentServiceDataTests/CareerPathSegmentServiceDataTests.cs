using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.ProfileService.SegmentServices;
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
    [Trait("Profile Service", "Career Path Segment Service Data Tests")]
    public class CareerPathSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly CareerPathSegmentDataModel ExpectedResult = new CareerPathSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<CareerPathSegmentService> logger;
        private readonly CareerPathSegmentClientOptions careerPathSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public CareerPathSegmentServiceDataTests()
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
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await careerPathSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            CareerPathSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await careerPathSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}