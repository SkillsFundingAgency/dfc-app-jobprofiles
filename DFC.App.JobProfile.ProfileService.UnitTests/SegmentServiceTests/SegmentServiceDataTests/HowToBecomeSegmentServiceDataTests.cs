using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceDataTests
{
    [Trait("Profile Service", "How To Become Segment Service Data Tests")]
    public class HowToBecomeSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly HowToBecomeSegmentModel ExpectedResult = new HowToBecomeSegmentModel
        {
            Updated = DateTime.Parse(ExpectedUpdated),
        };

        private readonly ILogger<HowToBecomeSegmentService> logger;
        private readonly HowToBecomeSegmentClientOptions howToBecomeSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public HowToBecomeSegmentServiceDataTests()
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
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await howToBecomeSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.Updated, ExpectedResult.Updated);
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
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await howToBecomeSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
