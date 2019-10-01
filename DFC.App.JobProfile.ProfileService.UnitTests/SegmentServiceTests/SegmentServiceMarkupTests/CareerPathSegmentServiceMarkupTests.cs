using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathDataModels;
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
    [Trait("Profile Service", "Career Path Segment Service Markup Tests")]
    public class CareerPathSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly CareerPathSegmentModel ExpectedResult = new CareerPathSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<CareerPathSegmentService> logger;
        private readonly CareerPathSegmentClientOptions careerPathSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(CareerPathSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public CareerPathSegmentServiceMarkupTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await careerPathSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            CareerPathSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await careerPathSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
