using AutoMapper;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using System;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests;

[Trait("Profile Controller", "Profile Tests")]
public class ProfileControllerTests
{
    [Fact]
    public void ProfileControllerWithNullContentIdThrows()
    {
        // Arrange
        var fakeLogger = A.Fake<ILogService>();
        var fakeJobProfileService = A.Fake<IJobProfileService>();
        var fakeMapper = A.Fake<IMapper>();
        var dummyConfigValues = A.Dummy<ConfigValues>();
        var feedbackLinks = A.Fake<FeedbackLinks>();
        var fakeSegmentService = A.Fake<ISegmentService>();
        var fakeRedirectionSecurityService = A.Fake<IRedirectionSecurityService>();
        var fakeStaticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new ProfileController(
                fakeLogger,
                fakeJobProfileService,
                fakeMapper,
                dummyConfigValues,
                feedbackLinks,
                fakeSegmentService,
                fakeRedirectionSecurityService,
                fakeStaticContentDocumentService,
                new CmsApiClientOptions()));

        // Assert
        Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
    }
}