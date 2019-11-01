using DFC.App.JobProfile.Data.Models;
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
            //new object[] { $"/profile/{DataSeeding.DefaultArticleName}/breadcrumb" },
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
        public async Task GetProfileHtmlContentEndpointsReturnNotFound(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PatchProfileEndpointsForNewArticleMetaDataPatchReturnOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToUpperInvariant();
            const string postUrl = "/profile";
            string patchUrl = $"/profile/{documentId}/metadata";
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToUpperInvariant(),
                SocLevelTwo = 33,
                LastReviewed = DateTime.UtcNow,
            };
            var jobProfileMetaDataPatchModel = new JobProfileModel()
            {
                CanonicalName = canonicalName,
                LastReviewed = DateTime.UtcNow,
                BreadcrumbTitle = "This is my breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTags
                {
                    Title = $"This is a title for {canonicalName}",
                    Description = "This is a description",
                    Keywords = "some keywords or other",
                },
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            using (var request = new HttpRequestMessage(HttpMethod.Patch, patchUrl))
            {
                jobProfileMetaDataPatchModel.SequenceNumber++;
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                request.Content = new ObjectContent(typeof(JobProfileModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

                // Act
                var response = await client.SendAsync(request).ConfigureAwait(false);

                // Assert
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.AlreadyReported);
            }
        }

        [Fact]
        public async Task PatchProfileEndpointsForNewArticleMetaDataPatchReturnNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToUpperInvariant();
            string url = $"/profile/{documentId}/metadata";
            var jobProfileMetaDataPatchModel = new JobProfileModel()
            {
                CanonicalName = canonicalName,
                LastReviewed = DateTime.Now,
                BreadcrumbTitle = "This is my breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTags
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
            request.Content = new ObjectContent(typeof(JobProfileModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

            // Act
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /* ***********
         * Integration test with segment apps yet to be finalised on approach
         * ************
        [Fact]
        public async Task PutProfileEndpointsForNewArticleRefreshOverviewBannerReturnOk()
        {
            // Arrange
            const string postUrl = "/profile";
            const string putUrl = "/refresh";
            var documentId = Guid.NewGuid();
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToUpperInvariant(),
                SocLevelTwo = 12,
                LastReviewed = DateTime.UtcNow,
            };
            var refreshJobProfileSegmentModel = new RefreshJobProfileSegment()
            {
                JobProfileId = jobProfileModel.DocumentId,
                CanonicalName = jobProfileModel.CanonicalName,
                Segment = Data.JobProfileSegment.Overview,
                LastReviewed = DateTime.UtcNow,
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
        */

        [Fact]
        public async Task DeleteProfileEndpointsReturnSuccessWhenFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            const string postUrl = "/profile";
            var deleteUri = new Uri($"/profile/{documentId}", UriKind.Relative);
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToUpperInvariant(),
                SocLevelTwo = 12,
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