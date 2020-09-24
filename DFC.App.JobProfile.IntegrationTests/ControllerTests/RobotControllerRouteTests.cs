using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.IntegrationTests.ControllerTests
{
    [Trait("Category", "Robot Controller Tests")]
    public class RobotControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        //private readonly CustomWebApplicationFactory<Startup> factory;

        //public RobotControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        //{
        //    this.factory = factory;
        //}

        //public static IEnumerable<object[]> RobotRouteData => new List<object[]>
        //{
        //    new object[] { "/robots.txt" },
        //};

        //[Theory]
        //[MemberData(nameof(RobotRouteData))]
        //public async Task GetRobotTextContentEndpointsReturnSuccessAndCorrectContentType(string url)
        //{
        //    // Arrange
        //    var uri = new Uri(url, UriKind.Relative);
        //    var client = factory.CreateClient();
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));

        //    // Act
        //    var response = await client.GetAsync(uri).ConfigureAwait(false);

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.ToString());
        //}
    }
}