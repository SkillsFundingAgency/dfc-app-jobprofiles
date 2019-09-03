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
    [Trait("Profile Service", "What It Takes Segment Service Tests")]
    public class WhatItTakesSegmentServiceTests
    {
        private const string ExpectedLastReviewed = "2019-08-30T08:00:00";
        private static readonly WhatItTakesSegmentModel ExpectedResult = new WhatItTakesSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpectedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<WhatItTakesSegmentService> logger;
        private readonly WhatItTakesSegmentClientOptions whatItTakesSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpectedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public WhatItTakesSegmentServiceTests()
        {
            logger = A.Fake<ILogger<WhatItTakesSegmentService>>();
            whatItTakesSegmentClientOptions = A.Fake<WhatItTakesSegmentClientOptions>();

            whatItTakesSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            whatItTakesSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task WhatItTakesSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatItTakesSegmentService = new WhatItTakesSegmentService(httpClient, logger, whatItTakesSegmentClientOptions);

                    // act
                    var results = await whatItTakesSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task WhatItTakesSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            WhatItTakesSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatItTakesSegmentService = new WhatItTakesSegmentService(httpClient, logger, whatItTakesSegmentClientOptions);

                    // act
                    var results = await whatItTakesSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
