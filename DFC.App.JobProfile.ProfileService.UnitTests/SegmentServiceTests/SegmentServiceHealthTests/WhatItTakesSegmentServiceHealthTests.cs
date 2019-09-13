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
    [Trait("Profile Service", "What It Takes Segment Service Health Tests")]
    public class WhatItTakesSegmentServiceHealthTests
    {
        private readonly ILogger<WhatItTakesSegmentService> logger;
        private readonly WhatItTakesSegmentClientOptions whatItTakesSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.WhatItTakes\",\"message\":\"Document store is available\"}]}";

        public WhatItTakesSegmentServiceHealthTests()
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
                    var results = await whatItTakesSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.Count, 1);
                }
            }
        }

        [Fact]
        public async Task WhatItTakesSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatItTakesSegmentService = new WhatItTakesSegmentService(httpClient, logger, whatItTakesSegmentClientOptions);

                    // act
                    var results = await whatItTakesSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}