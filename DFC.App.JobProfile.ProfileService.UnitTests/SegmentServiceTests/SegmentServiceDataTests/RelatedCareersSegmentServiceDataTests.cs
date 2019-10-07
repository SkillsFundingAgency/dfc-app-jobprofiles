﻿using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
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
    [Trait("Profile Service", "Related Careers Segment Service Data Tests")]
    public class RelatedCareersSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly RelatedCareersSegmentDataModel ExpectedResult = new RelatedCareersSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<RelatedCareersSegmentService> logger;
        private readonly RelatedCareersSegmentClientOptions relatedCareersSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public RelatedCareersSegmentServiceDataTests()
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
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await relatedCareersSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task RelatedCareersSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            RelatedCareersSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var relatedCareersSegmentService = new RelatedCareersSegmentService(httpClient, logger, relatedCareersSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await relatedCareersSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}
