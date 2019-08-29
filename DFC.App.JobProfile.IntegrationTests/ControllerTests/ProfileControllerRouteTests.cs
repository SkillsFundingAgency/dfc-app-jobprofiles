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
    [Trait("Integration Tests", "profile Controller Tests")]
    public class ProfileControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const string DefaultArticleName = "profile-article";

        private readonly CustomWebApplicationFactory<Startup> factory;

        public ProfileControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        public static IEnumerable<object[]> ProfileContentRouteData => new List<object[]>
        {
            new object[] { "/profile" },
            new object[] { $"/profile/{DefaultArticleName}" },
            new object[] { $"/profile/{DefaultArticleName}/htmlhead" },
            new object[] { $"/profile/{DefaultArticleName}/breadcrumb" },
            new object[] { $"/profile/{DefaultArticleName}/contents" },
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
        public async Task PostHelpEndpointsReturnCreated()
        {
            // Arrange
            const string url = "/profile";
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel()
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = Guid.NewGuid().ToString().ToLowerInvariant(),
                RefreshAllSegments = true,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, createOrUpdateJobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PuttHelpEndpointsReturnOk()
        {
            // Arrange
            const string url = "/profile";
            var createOrUpdateJobProfileModel = new CreateOrUpdateJobProfileModel()
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = Guid.NewGuid().ToString().ToLowerInvariant(),
                RefreshOverviewBannerSegment = true,
                RefreshRelatedCareersSegment = true,
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(url, createOrUpdateJobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.PutAsync(url, createOrUpdateJobProfileModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteHelpEndpointsReturnNotFound()
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