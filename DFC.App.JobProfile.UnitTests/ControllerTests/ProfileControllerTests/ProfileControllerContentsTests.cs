using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Contents Tests")]
    public class ProfileControllerContentsTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void JobProfileControllerContentsHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = controller.Body();

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            viewResult.Url.Should().Be("/explore-careers");
            viewResult.Permanent.Should().BeFalse();

            controller.Dispose();
        }
    }
}
