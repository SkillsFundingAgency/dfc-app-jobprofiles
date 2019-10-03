using DFC.App.JobProfile.Data.Models.ServiceBusModels;
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
        public async Task PostProfileEndpointsReturnCreated()
        {
            // Arrange
            const string url = "/profile";
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegment = new RefreshJobProfileSegment()
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
                Segment = null,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PostProfileEndpointsForDefaultArticleRefreshAllReturnOk()
        {
            // Arrange
            const string url = "/profile";
            var refreshJobProfileSegment = new RefreshJobProfileSegment()
            {
                JobProfileId = DataSeeding.DefaultArticleGuid,
                CanonicalName = DataSeeding.DefaultArticleName,
                SocLevelTwo = "12",
                Segment = "none",
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PutProfileEndpointsForDefaultArticleRefreshAllReturnOk()
        {
            // Arrange
            const string url = "/profile";
            var refreshJobProfileSegment = new RefreshJobProfileSegment()
            {
                JobProfileId = DataSeeding.DefaultArticleGuid,
                CanonicalName = DataSeeding.DefaultArticleName,
                SocLevelTwo = "12",
                Segment = "none",
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PutAsync(url, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PutProfileEndpointsReturnOk()
        {
            // Arrange
            const string url = "/profile";
            var documentId = Guid.NewGuid();
            var refreshJobProfileSegment = new RefreshJobProfileSegment()
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
                Segment = "OverviewBanner",
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(url, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.PutAsync(url, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

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
            var refreshJobProfileSegment = new RefreshJobProfileSegment()
            {
                JobProfileId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12",
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, refreshJobProfileSegment, new JsonMediaTypeFormatter()).ConfigureAwait(false);

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