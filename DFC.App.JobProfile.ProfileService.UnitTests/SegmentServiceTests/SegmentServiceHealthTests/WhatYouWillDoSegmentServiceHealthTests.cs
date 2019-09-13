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
    [Trait("Profile Service", "What You Will Do Segment Service Health Tests")]
    public class WhatYouWillDoSegmentServiceHealthTests
    {
        private readonly ILogger<WhatYouWillDoSegmentService> logger;
        private readonly WhatYouWillDoSegmentClientOptions whatYouWillDoSegmentClientOptions;

        private readonly string responseJson = "{\"healthItems\":[{\"service\":\"DFC.App.WhatYouWillDo\",\"message\":\"Document store is available\"}]}";

        public WhatYouWillDoSegmentServiceHealthTests()
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatYouWillDoSegmentService = new WhatYouWillDoSegmentService(httpClient, logger, whatYouWillDoSegmentClientOptions);

                    // act
                    var results = await whatYouWillDoSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.Count, 1);
                }
            }
        }

        [Fact]
        public async Task WhatYouWillDoSegmentServiceReturnsNullWhenError()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatYouWillDoSegmentService = new WhatYouWillDoSegmentService(httpClient, logger, whatYouWillDoSegmentClientOptions);

                    // act
                    var results = await whatYouWillDoSegmentService.HealthCheckAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, null);
                }
            }
        }
    }
}