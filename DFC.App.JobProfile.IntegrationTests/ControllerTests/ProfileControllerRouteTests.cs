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
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}/hero" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleName}/contents" },
            new object[] { $"/profile/{DataSeeding.DefaultArticleGuid}/profile" },
        };

        public static IEnumerable<object[]> ProfileNoContentRouteData => new List<object[]>
        {
            new object[] { $"/profile/htmlhead" },
            new object[] { $"/profile/hero" },
        };

        public static IEnumerable<object[]> MissingProfileContentRouteData => new List<object[]>
        {
            new object[] { $"/profile/invalid-profile-name" },
        };

        public static IEnumerable<object[]> ProfileContentRedirectRouteData => new List<object[]>
        {
            new object[] { $"/profile/contents" },
            new object[] { $"/profile/search/action" },
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
        [MemberData(nameof(ProfileNoContentRouteData))]
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

        [Theory]
        [MemberData(nameof(MissingProfileContentRouteData))]
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

        [Theory]
        [MemberData(nameof(ProfileContentRedirectRouteData))]
        public async Task GetProfileHtmlContentEndpointsReturnRedirection(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task PostProfileEndpointsForNewArticleMetaDataReturnsOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToUpperInvariant();
            const string postUrl = "/profile";
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                JobProfileId = documentId,
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
                SequenceNumber = 1,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var response = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostProfileEndpointsForNewArticleMetaDataReturnsAlreadyReported()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToUpperInvariant();
            const string postUrl = "/profile";
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                JobProfileId = documentId,
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
                SequenceNumber = 1,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var response1 = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);
            var response2 = await client.PostAsync(postUrl, jobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.AlreadyReported);
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
                JobProfileId = documentId,
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
                SequenceNumber = 1,
                SocLevelTwo = "21",
            };

            var jobProfileMetaDataPatchModel = new JobProfileModel()
            {
                JobProfileId = documentId,
                CanonicalName = canonicalName,
                LastReviewed = DateTime.UtcNow,
                BreadcrumbTitle = "This is my patched breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTags
                {
                    Title = $"This is a patch title for {canonicalName}",
                    Description = "This is a patch description",
                    Keywords = "some keywords or other",
                },
                SequenceNumber = 2,
                SocLevelTwo = "21",
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
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task PatchProfileEndpointsForNewArticleMetaDataPatchReturnsAlreadyReported()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            string canonicalName = documentId.ToString().ToUpperInvariant();
            const string postUrl = "/profile";
            string patchUrl = $"/profile/{documentId}/metadata";
            var jobProfileModel = new Data.Models.JobProfileModel()
            {
                JobProfileId = documentId,
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
                SequenceNumber = 1,
            };
            var jobProfileMetaDataPatchModel = new JobProfileModel()
            {
                JobProfileId = documentId,
                CanonicalName = canonicalName,
                LastReviewed = DateTime.UtcNow,
                BreadcrumbTitle = "This is my patched breadcrumb title",
                IncludeInSitemap = true,
                AlternativeNames = new string[] { "jp1", "jp2" },
                MetaTags = new MetaTags
                {
                    Title = $"This is a patch title for {canonicalName}",
                    Description = "This is a patch description",
                    Keywords = "some keywords or other",
                },
                SequenceNumber = 1,
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
                SocLevelTwo = "21",
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            using var request = new HttpRequestMessage(HttpMethod.Patch, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            request.Content = new ObjectContent(typeof(JobProfileModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

            // Act
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

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
                SocLevelTwo = "12",
                LastReviewed = DateTime.UtcNow,
                IncludeInSitemap = true,
                MetaTags = new MetaTags
                {
                    Title = $"This is a title",
                },
                SequenceNumber = 1,
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