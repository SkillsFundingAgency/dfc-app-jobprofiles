using AutoMapper;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
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
        var fakeConfiguration = A.Fake<IConfiguration>();
        var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();

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
        new CmsApiClientOptions(),
                fakeSharedContentRedisInterface, 
                fakeConfiguration));

        // Assert
        Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
    }
}