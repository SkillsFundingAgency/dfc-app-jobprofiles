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
    [Trait("Profile Service", "Related Careers Segment Service Tests")]
    public class RelatedCareersSegmentServiceTests
    {
        private const string ExpecedLastReviewed = "2019-08-30T08:00:00";
        private static readonly RelatedCareersSegmentModel ExpectedResult = new RelatedCareersSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpecedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<RelatedCareersSegmentService> logger;
        private readonly RelatedCareersSegmentClientOptions relatedCareersSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpecedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public RelatedCareersSegmentServiceTests()
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
                    var results = await relatedCareersSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task RelatedCareersSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            RelatedCareersSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions);

                    // act
                    var results = await relatedCareersSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
