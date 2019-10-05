﻿using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using DFC.App.JobProfile.ProfileService.SegmentServices;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.SegmentServiceTests.SegmentServiceDataTests
{
    [Trait("Profile Service", "What It Takes Segment Service Data Tests")]
    public class WhatItTakesSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly JobProfileSkillSegmentDataModel ExpectedResult = new JobProfileSkillSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<WhatItTakesSegmentService> logger;
        private readonly WhatItTakesSegmentClientOptions whatItTakesSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public WhatItTakesSegmentServiceDataTests()
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
                    var whatItTakesSegmentService = new WhatItTakesSegmentService(httpClient, logger, whatItTakesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await whatItTakesSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task WhatItTakesSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            JobProfileSkillSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var whatItTakesSegmentService = new WhatItTakesSegmentService(httpClient, logger, whatItTakesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await whatItTakesSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
