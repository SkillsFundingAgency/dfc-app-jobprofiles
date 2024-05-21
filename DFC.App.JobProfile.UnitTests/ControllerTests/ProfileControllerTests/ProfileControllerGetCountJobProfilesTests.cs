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
    public class ProfileControllerGetCountJobProfilesTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerGetCountJobProfilesReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            int num = 1;
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.CountJobProfiles(A<string>.Ignored)).Returns(num);

            // Act
            var result = await controller.CountJobProfiles();

            // Assert
            var statusCodeResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}