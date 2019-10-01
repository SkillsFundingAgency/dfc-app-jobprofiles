using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersDataModels;
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
    [Trait("Profile Service", "Related Careers Segment Service Markup Tests")]
    public class RelatedCareersSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly RelatedCareersSegmentModel ExpectedResult = new RelatedCareersSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<RelatedCareersSegmentService> logger;
        private readonly RelatedCareersSegmentClientOptions relatedCareersSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(RelatedCareersSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public RelatedCareersSegmentServiceMarkupTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await relatedCareersSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task RelatedCareersSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            RelatedCareersSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await relatedCareersSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
