using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.ProfileService.SegmentServices;
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
    [Trait("Profile Service", "How To Become Segment Service Markup Tests")]
    public class HowToBecomeSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly HowToBecomeSegmentDataModel ExpectedResult = new HowToBecomeSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<HowToBecomeSegmentService> logger;
        private readonly HowToBecomeSegmentClientOptions howToBecomeSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(HowToBecomeSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public HowToBecomeSegmentServiceMarkupTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await howToBecomeSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task HowToBecomeSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            HowToBecomeSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var howToBecomeSegmentService = new HowToBecomeSegmentService(httpClient, logger, howToBecomeSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await howToBecomeSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
