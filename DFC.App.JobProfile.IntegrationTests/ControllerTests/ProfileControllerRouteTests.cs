﻿using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.IntegrationTests.ControllerTests
{
    [Trait("Integration Tests", "Profile Controller Tests")]
    public class ProfileControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public ProfileControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticle(factory);
        }

        public static IEnumerable<object[]> ProfileContentRouteData => new List<object[]>
        {
            new object[] { "/profile" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}/htmlhead" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}/breadcrumb" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}/contents" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleGuid}/profile" },
        };

        public static IEnumerable<object[]> MissingprofileContentRouteData => new List<object[]>
        {
            new object[] { $"/profile/invalid-profile-name" },
        };

        [Theory]
        [MemberData(nameof(ProfileContentRouteData))]
        public async Task GetProfileHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(ProfileContentRouteData))]
        public async Task GetProfileJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(MissingprofileContentRouteData))]
        public async Task GetProfileHtmlContentEndpointsReturnNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PatchProfileEndpointsForNewArticleMetaDataPatchReturnOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToLowerInvariant();
            const string postUrl = "/profile";
            string patchUrl = $"/profile/{documentId}/metadata";
            var jobProfileModel = new JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "33",
                LastReviewed = DateTime.UtcNow,
            };
            var jobProfileMetaDataPatchModel = new JobProfileMetaDataPatchModel()
            {
                CanonicalName = canonicalName,
                BreadcrumbTitle = "This is my breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTagsModel
                {
                    Title = $"This is a title for {canonicalName}",
                    Description = "This is a description",
                    Keywords = "some keywords or other",
                },
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            var request = new HttpRequestMessage(HttpMethod.Patch, patchUrl);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            request.Content = new ObjectContent(typeof(JobProfileMetaDataPatchModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

            // Act
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PatchProfileEndpointsForNewArticleMetaDataPatchReturnNoContent()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToLowerInvariant();
            string url = $"/profile/{documentId}/metadata";
            var jobProfileMetaDataPatchModel = new JobProfileMetaDataPatchModel()
            {
                CanonicalName = canonicalName,
                BreadcrumbTitle = "This is my breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTagsModel
                {
                    Title = $"This is a title for {canonicalName}",
                    Description = "This is a description",
                    Keywords = "some keywords or other",
                },
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var request = new HttpRequestMessage(HttpMethod.Patch, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            request.Content = new ObjectContent(typeof(JobProfileMetaDataPatchModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

            // Act
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PostProfileEndpointsForDefaultArticleRefreshAllReturnOk()
        {
            // Arrange
            const string url = "/profile/refresh";
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegmentModel()
            {
                DocumentId = DataSeeding.DefaultArticleGuid,
                CanonicalName = DataSeeding.DefaultArticleName,
                Segment = null,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, refreshJobProfileSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostProfileEndpointsForNewArticleRefreshAllReturnOk()
        {
            // Arrange
            const string postUrl = "/profile";
            const string postRefreshUrl = "/profile/refresh";
            var documentId = Guid.NewGuid();
            var jobProfileModel = new JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
                LastReviewed = DateTime.UtcNow,
            };
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegmentModel()
            {
                DocumentId = jobProfileModel.DocumentId,
                CanonicalName = jobProfileModel.CanonicalName,
                Segment = null,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.PostAsync(postRefreshUrl, refreshJobProfileSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PutProfileEndpointsForNewArticleRefreshOverviewBannerReturnOk()
        {
            // Arrange
            const string postUrl = "/profile";
            const string putUrl = "/profile/refresh";
            var documentId = Guid.NewGuid();
            var jobProfileModel = new JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
                LastReviewed = DateTime.UtcNow,
            };
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegmentModel()
            {
                DocumentId = jobProfileModel.DocumentId,
                CanonicalName = jobProfileModel.CanonicalName,
                Segment = OverviewBannerSegmentDataModel.SegmentName,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.PutAsync(putUrl, refreshJobProfileSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteProfileEndpointsReturnSuccessWhenFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            const string postUrl = "/profile";
            var deleteUri = new Uri($"/profile/{documentId}", UriKind.Relative);
            var jobProfileModel = new JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
                LastReviewed = DateTime.UtcNow,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.DeleteAsync(deleteUri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteProfileEndpointsReturnNotFound()
        {
            // Arrange
            var uri = new Uri($"/profile/{Guid.NewGuid()}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}