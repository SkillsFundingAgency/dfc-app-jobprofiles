using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels;
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
    [Trait("Profile Service", "What You Will Do Segment Service Markup Tests")]
    public class WhatYouWillDoSegmentServiceMarkupTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly JobProfileTasksSegmentDataModel ExpectedResult = new JobProfileTasksSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
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
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await whatYouWillDoSegmentService.LoadMarkupAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task WhatYouWillDoSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            JobProfileTasksSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseHtml, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatYouWillDoSegmentService = new WhatYouWillDoSegmentService(httpClient, logger, whatYouWillDoSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
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
