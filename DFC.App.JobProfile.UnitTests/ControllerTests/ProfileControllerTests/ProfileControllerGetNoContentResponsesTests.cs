using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Create or Update Tests")]
    public class ProfileControllerGetNoContentResponsesTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerGetNoContentResponsesReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            int num = 1;
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = controller.NoContentResponses();

            // Assert
            var statusCodeResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}