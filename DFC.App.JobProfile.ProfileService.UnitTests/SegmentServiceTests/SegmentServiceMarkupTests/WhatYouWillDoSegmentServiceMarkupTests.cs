using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceMarkupTests
{
    [Trait("Profile Service", "What You Will Do Segment Service Markup Tests")]
    public class WhatYouWillDoSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly WhatYouWillDoSegmentModel ExpectedResult = new WhatYouWillDoSegmentModel
        {
            Updated = DateTime.Parse(ExpectedUpdated),
        };

        private readonly ILogger<WhatYouWillDoSegmentService> logger;
        private readonly WhatYouWillDoSegmentClientOptions whatYouWillDoSegmentClientOptions;

        private readonly string responseHtml = $"<div><h1>{nameof(WhatYouWillDoSegmentServiceMarkupTests)}</h1><p>Lorum ipsum ...</p></div>";

        public WhatYouWillDoSegmentServiceMarkupTests()
        {
            logger = A.Fake<ILogger<WhatYouWillDoSegmentService>>();
            whatYouWillDoSegmentClientOptions = A.Fake<WhatYouWillDoSegmentClientOptions>();

            whatYouWillDoSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            whatYouWillDoSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task WhatYouWillDoSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatYouWillDoSegmentService = new WhatYouWillDoSegmentService(httpClient, logger, whatYouWillDoSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await whatYouWillDoSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.Updated);
                }
            }
        }

        [Fact]
        public async Task WhatYouWillDoSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            WhatYouWillDoSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatYouWillDoSegmentService = new WhatYouWillDoSegmentService(httpClient, logger, whatYouWillDoSegmentClientOptions)
                    {
                        CanonicalName = "article-name",
                    };

                    // act
                    var results = await whatYouWillDoSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
