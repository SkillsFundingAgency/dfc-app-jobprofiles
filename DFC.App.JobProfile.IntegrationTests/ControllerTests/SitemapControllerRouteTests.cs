using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using Moq;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.IntegrationTests.ControllerTests
{
    [Trait("Category", "Sitemap Controller Tests")]
    public class SitemapControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public SitemapControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticle(factory);
        }

        public static IEnumerable<object[]> SitemapRouteData => new List<object[]>
        {
            new object[] { "/sitemap.xml" },
        };

        [Theory]
        [MemberData(nameof(SitemapRouteData))]
        public async Task GetSitemapXmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));

            var jobProfileCurrentOpportunitiesResponse = new JobProfileCurrentOpportunitiesResponse()
            {
                JobProfileCurrentOpportunities = new List<JobProfileCurrentOpportunities>() { new JobProfileCurrentOpportunities() { DisplayText = "test" } },
            };

            A.CallTo(() => factory.FakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(jobProfileCurrentOpportunitiesResponse);

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(MediaTypeNames.Application.Xml, response.Content.Headers.ContentType.ToString());
        }
    }
}