using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
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
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            // Act
            var result = controller.Contents();

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            viewResult.Url.Should().Be("/explore-careers");
            viewResult.Permanent.Should().BeFalse();

            controller.Dispose();
        }
    }
}
