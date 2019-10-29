﻿using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
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
    [Trait("Profile Service", "Current Opportunities Segment Service Data Tests")]
    public class CurrentOpportunitiesSegmentServiceDataTests
    {
        private const string ExpectedUpdated = "2019-08-30T08:00:00";
        private static readonly CurrentOpportunitiesSegmentDataModel ExpectedResult = new CurrentOpportunitiesSegmentDataModel
        {
            LastReviewed = DateTime.Parse(ExpectedUpdated, CultureInfo.InvariantCulture),
        };

        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly CurrentOpportunitiesSegmentClientOptions currentOpportunitiesSegmentClientOptions;

        private readonly string responseJson = $"{{\"Updated\": \"{ExpectedUpdated}\"}}";

        public CurrentOpportunitiesSegmentServiceDataTests()
        {
            logger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            currentOpportunitiesSegmentClientOptions = A.Fake<CurrentOpportunitiesSegmentClientOptions>();

            currentOpportunitiesSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            currentOpportunitiesSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                }
            }
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            CurrentOpportunitiesSegmentDataModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(httpClient, logger, currentOpportunitiesSegmentClientOptions)
                    {
                        DocumentId = Guid.NewGuid(),
                    };

                    // act
                    var results = await currentOpportunitiesSegmentService.LoadDataAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}