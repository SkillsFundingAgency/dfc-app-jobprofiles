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
        };

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

        [Theory(Skip ="Temporary")]
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
    }
}