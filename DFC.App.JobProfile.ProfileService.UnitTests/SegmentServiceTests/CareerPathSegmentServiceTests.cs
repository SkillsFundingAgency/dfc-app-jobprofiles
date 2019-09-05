﻿using DFC.App.JobProfile.Data.HttpClientPolicies;
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
    [Trait("Profile Service", "Career Path Segment Service Tests")]
    public class CareerPathSegmentServiceTests
    {
        private const string ExpecedLastReviewed = "2019-08-30T08:00:00";
        private static readonly CareerPathSegmentModel ExpectedResult = new CareerPathSegmentModel
        {
            LastReviewed = DateTime.Parse(ExpecedLastReviewed),
            Content = "some content",
        };

        private readonly ILogger<CareerPathSegmentService> logger;
        private readonly CareerPathSegmentClientOptions careerPathSegmentClientOptions;

        private readonly string responseJson = $"{{\"LastReviewed\": \"{ExpecedLastReviewed}\", \"Content\": \"{ExpectedResult.Content}\"}}";

        public CareerPathSegmentServiceTests()
        {
            logger = A.Fake<ILogger<CareerPathSegmentService>>();
            careerPathSegmentClientOptions = A.Fake<CareerPathSegmentClientOptions>();

            careerPathSegmentClientOptions.BaseAddress = new Uri("https://nowhere.com");
            careerPathSegmentClientOptions.Endpoint = "segment/{0}/contents";
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsSuccessWhenOK()
        {
            // arrange
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions);

                    // act
                    var results = await careerPathSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results.LastReviewed, ExpectedResult.LastReviewed);
                    A.Equals(results.Content, ExpectedResult.Content);
                }
            }
        }

        [Fact]
        public async Task CareerPathSegmentServiceReturnsNullWhenNotFound()
        {
            // arrange
            const string responseJson = "{\"notValid\": true}";
            CareerPathSegmentModel expectedResult = null;

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(responseJson, HttpStatusCode.NotFound))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var careerPathSegmentService = new CareerPathSegmentService(httpClient, logger, careerPathSegmentClientOptions);

                    // act
                    var results = await careerPathSegmentService.LoadAsync("article-name").ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResult);
                }
            }
        }
    }
}