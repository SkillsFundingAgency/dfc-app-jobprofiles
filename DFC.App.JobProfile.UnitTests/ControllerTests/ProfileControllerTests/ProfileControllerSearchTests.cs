using DFC.App.JobProfile.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "HTML Search Tests")]
    public class ProfileControllerSearchTests : BaseProfileController
    {
        [Fact]
        public void JobProfileControllerSearchRedirectsToSearchResults()
        {
            // Arrange
            const string jobProfileUrl = "an-article";
            const string searchTerm = "search term";
            var controller = BuildProfileController(MediaTypeNames.Text.Html);

            // Act
            var result = controller.Search(jobProfileUrl, searchTerm);

            // Assert
            var statusResult = Assert.IsType<RedirectResult>(result);

            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            statusResult.Url.Should().StartWith("/search-results?");
            A.Equals(false, statusResult.Permanent);

            controller.Dispose();
        }

        [Fact]
        public void JobProfileControllerSearchRedirectsToSelfForNullSearchTerm()
        {
            // Arrange
            const string jobProfileUrl = "an-article";
            const string searchTerm = null;
            var controller = BuildProfileController(MediaTypeNames.Text.Html);

            // Act
            var result = controller.Search(jobProfileUrl, searchTerm);

            // Assert
            var statusResult = Assert.IsType<RedirectResult>(result);

            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            statusResult.Url.Should().StartWith($"/{ProfileController.ProfilePathRoot}/");
            A.Equals(false, statusResult.Permanent);

            controller.Dispose();
        }
    }
}
